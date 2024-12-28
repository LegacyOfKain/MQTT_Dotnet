using MQTTnet;
using MQTTnet.Client;
using SparkplugNet.VersionA.Data;
using SparkplugNet.VersionB.Data;

var factory = new MqttFactory();
var mqttClient = factory.CreateMqttClient();

var mqttOptions = new MqttClientOptionsBuilder()
    .WithTcpServer("broker.hivemq.com", 1883)
    .Build();

await mqttClient.ConnectAsync(mqttOptions, CancellationToken.None);

// Define Sparkplug topic components
var namespace = "spBv1.0";
var groupId = "MyGroup";
var edgeNodeId = "NDEdgeNode";
var deviceId = "Device1";

var topicGenerator = new SparkplugTopicGenerator();

while (true)
{
    // Create a Sparkplug B payload
    var payload = new PayloadBuilder()
        .AddMetric(new Metric
        {
            Name = "Temperature",
            Value = Random.Shared.NextDouble() * 100, // Random temperature between 0 and 100
            Type = (uint)DataType.Double
        })
        .CreatePayload();

// Encode the payload
var encodedPayload = SparkplugMessageSerializer.SerializePayload(payload);

// Generate the full Sparkplug B topic
var topic = topicGenerator.GenerateDeviceDataTopic(namespace, groupId, edgeNodeId, deviceId);

    var message = new MqttApplicationMessageBuilder()
        .WithTopic(topic)
        .WithPayload(encodedPayload)
        .Build();

await mqttClient.PublishAsync(message, CancellationToken.None);

Console.WriteLine($"Metric published. Topic: {topic}");
    await Task.Delay(1000); // Wait for 1 second before sending the next message
}

Console.ReadLine();