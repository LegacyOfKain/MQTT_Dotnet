using MQTTnet;

var clientCount = 3;
var messagePerClient = 100000;

var tasks = new List<Task>();

for (int clientId = 1; clientId <= clientCount; clientId++)
{
    int localClientId = clientId; // This is a common closure issue in C#. Creating a local copy inside the loop is the recommended solution to avoid reference
    tasks.Add(Task.Run(async () =>
    {
        var mqttFactory = new MqttClientFactory();
        var mqttClient = mqttFactory.CreateMqttClient();
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("broker.hivemq.com", 8883)
            .WithTlsOptions(o => o.UseTls())
            .WithClientId($"ProducerClient_{localClientId}")
            .Build();

        try
        {
            var connectResult = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
            Console.WriteLine($"Client {localClientId} connection result: {connectResult.ResultCode}");

            if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
            {
                Console.WriteLine($"Client {localClientId} connected successfully");

                for (int i = 1; i <= messagePerClient; i++)
                {
                    var payload = $"Client {localClientId} - Message {i}: {Random.Shared.Next(100)} {DateTime.UtcNow:O}";
                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic("topic/MyTopicDemo481")
                        .WithPayload(payload)
                        .Build();

                    var publishResult = await mqttClient.PublishAsync(message, CancellationToken.None);
                    Console.WriteLine($"Client {localClientId} published: {payload} | Reason: {publishResult.ReasonCode}");
                }

                await mqttClient.DisconnectAsync();
                Console.WriteLine($"Client {localClientId} disconnected");
            }
            else
            {
                Console.WriteLine($"Client {localClientId} failed to connect. Reason: {connectResult.ResultCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Client {localClientId} error: {ex.Message}");
        }
    }));
}

await Task.WhenAll(tasks);

Console.WriteLine("All clients finished. Press any key to exit...");
Console.ReadKey();