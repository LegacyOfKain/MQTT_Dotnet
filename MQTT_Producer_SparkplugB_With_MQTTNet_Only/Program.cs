﻿using Google.Protobuf.Reflection;
using Google.Protobuf;
using MQTTnet;
using MQTTnet.Client;
using System.Text;

var factory = new MqttFactory();
var mqttClient = factory.CreateMqttClient();

var mqttOptions = new MqttClientOptionsBuilder()
    .WithTcpServer("broker.hivemq.com", 1883)
    .Build();

await mqttClient.ConnectAsync(mqttOptions, CancellationToken.None);

// Define Sparkplug topic and payload
var topic = "spBv1.0/MyGroup/NDEdgeNode/DATA";


while (true)
{
    //var payloadString = "{ \"Metric\": { \"Name\": \"Temperature\", \"Value\": "+Random.Shared.NextDouble()+" } }";
    //var payload = Encoding.UTF8.GetBytes(payloadString);
    var payload = getPayloadData();
    var message = new MqttApplicationMessageBuilder()
        .WithTopic(topic)
        .WithPayload(payload)
        .Build();

    await mqttClient.PublishAsync(message, CancellationToken.None);

    Console.WriteLine("Metric published.");
    await Task.Delay(1000); // Wait for 1 second before sending the next message
}

byte[] getPayloadData()
{
    // Generate some sample data to publish
    var sensorData = new SensorData
    {
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        SensorId = "temperature_sensor_1",
        Value = 22.5f
    };

    // Serialize using Google Protobuf
    var serializedData = sensorData.ToByteArray();

    return serializedData;
}

Console.ReadLine();

public class SensorData : IMessage<SensorData>
{
    private static readonly MessageParser<SensorData> _parser = new MessageParser<SensorData>(() => new SensorData());
    public static MessageParser<SensorData> Parser => _parser;

    public long Timestamp { get; set; }
    public string SensorId { get; set; } = "";
    public float Value { get; set; }

    public MessageDescriptor Descriptor => throw new NotImplementedException();

    public void MergeFrom(CodedInputStream input)
    {
        while (input.ReadTag() is var tag && tag != 0)
        {
            switch (tag)
            {
                case 8: // Timestamp
                    Timestamp = input.ReadInt64();
                    break;
                case 18: // SensorId
                    SensorId = input.ReadString();
                    break;
                case 29: // Value
                    Value = input.ReadFloat();
                    break;
                default:
                    input.SkipLastField();
                    break;
            }
        }
    }

    public void WriteTo(CodedOutputStream output)
    {
        output.WriteRawTag(8);  // Tag for field number 1, wire type 0
        output.WriteInt64(Timestamp);

        output.WriteRawTag(18);  // Tag for field number 2, wire type 2
        output.WriteString(SensorId);

        output.WriteRawTag(29);  // Tag for field number 3, wire type 5
        output.WriteFloat(Value);
    }

    /*
        1. We calculate the size for each field separately.
        2. For each field, we add 1 byte for the tag (which includes the field number and wire type).
        3. For the Timestamp (Int64) and SensorId (String), we use the appropriate ComputeXXXSize method.
        4. For the Value (Float), we simply add 4 bytes as floats are always 4 bytes in size.
    */
    public int CalculateSize()
    {
        int size = 0;
        size += 1 + CodedOutputStream.ComputeInt64Size(Timestamp);
        size += 1 + CodedOutputStream.ComputeStringSize(SensorId);
        size += 1 + 4;  // 1 byte for tag, 4 bytes for float
        return size;
    }

    public SensorData Clone() => new SensorData
    {
        Timestamp = Timestamp,
        SensorId = SensorId,
        Value = Value
    };

    public void MergeFrom(SensorData message)
    {
        if (message == null)
            return;

        if (message.Timestamp != 0)
            Timestamp = message.Timestamp;

        if (!string.IsNullOrEmpty(message.SensorId))
            SensorId = message.SensorId;

        if (message.Value != 0)
            Value = message.Value;
    }

    public bool Equals(SensorData? other)
    {
        if (other == null)
            return false;

        return Timestamp == other.Timestamp &&
               SensorId == other.SensorId &&
               Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as SensorData);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Timestamp, SensorId, Value);
    }
}