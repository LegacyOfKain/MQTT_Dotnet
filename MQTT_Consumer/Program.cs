using MQTTnet;

int messageCount = 0; // Add this at the top of your try block

try
{
    var mqttFactory = new MqttClientFactory();
    var mqttClient = mqttFactory.CreateMqttClient();
    var mqttClientOptions = new MqttClientOptionsBuilder()
        .WithTcpServer("broker.hivemq.com", 8883)  // 8883 is the default MQTT TLS port
        .WithClientId("MyConsumerClient")
        .WithCleanStart()
        .WithKeepAlivePeriod(TimeSpan.FromSeconds(30)) // this is very important for mqtt.net v5
        .WithTlsOptions(o => o.UseTls())
        .Build();

    mqttClient.ApplicationMessageReceivedAsync += e =>
    {
        Interlocked.Increment(ref messageCount); // Thread-safe increment
        Console.WriteLine($"Received message #{messageCount}: {e.ApplicationMessage.ConvertPayloadToString}");
        return Task.CompletedTask;
    };

    mqttClient.DisconnectedAsync += async e =>
    {
        Console.WriteLine("Disconnected from the broker. Attempting to reconnect...");
        await Task.Delay(TimeSpan.FromSeconds(5));
        try
        {
            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
        }
        catch
        {
            Console.WriteLine("Reconnection failed. Will try again later.");
        }
    };

    await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

    await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
        .WithTopic("topic/MyTopicDemo481")
        .Build());

    Console.WriteLine("MQTT Consumer started. Press any key to exit.");
    Console.ReadKey();

    await mqttClient.DisconnectAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"An unhandled error occurred: {ex.Message}");
}