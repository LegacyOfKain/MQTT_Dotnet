using MQTTnet;
using MQTTnet.Client;

try
{
    var mqttFactory = new MqttFactory();
    var mqttClient = mqttFactory.CreateMqttClient();
    var mqttClientOptions = new MqttClientOptionsBuilder()
        .WithTcpServer("broker.hivemq.com", 8883)  // 8883 is the default MQTT TLS port
        .WithTlsOptions(o => o.UseTls())
        .Build();

    var connectResult = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
    Console.WriteLine($"Connection result: {connectResult.ResultCode}");

    if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
    {
        Console.WriteLine("Connected to MQTT broker successfully");

        for (int i = 1; i <= 10; i++)
        {
            var payload = $"Hello MQTT! Message {i}: {Random.Shared.Next(100)}";

            var message = new MqttApplicationMessageBuilder()
                .WithTopic("topic/MyTopicDemo481")
                .WithPayload(payload)
                .Build();

            var publishResult = await mqttClient.PublishAsync(message, CancellationToken.None);
            Console.WriteLine($"Message published = {payload} \n Reason: {publishResult.ReasonCode}");

            if (i < 10) // Don't delay after the last message
            {
                await Task.Delay(1000); // Wait for 1 second before sending the next message
            }
        }

        await mqttClient.DisconnectAsync();
        Console.WriteLine("Disconnected from MQTT broker");
    }
    else
    {
        Console.WriteLine($"Failed to connect. Reason: {connectResult.ResultCode}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();