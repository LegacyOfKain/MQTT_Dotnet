using SparkplugNet.Core.Application;
using SparkplugNet.Core.Enumerations;

using SparkplugNet.Core;
using SparkplugNet.VersionB.Data;

using VersionBData = SparkplugNet.VersionB.Data;
using Core = SparkplugNet.Core;
using VersionB = SparkplugNet.VersionB;



CancellationTokenSource CancellationTokenSource = new();

/// <summary>
/// The version B metrics for an application.
/// </summary>
List<VersionBData.Metric> VersionBMetricsApplication =
[
    new VersionBData.Metric(
        name: "temperatureApplication",
        dataType: VersionBData.DataType.Float,
        value: 1.20f
    ),
    new VersionBData.Metric(
        name: "climateactiveApplication",
        dataType: VersionBData.DataType.Boolean,
        value: true
    )
    {
        Properties = new VersionBData.PropertySet
        {
            Keys = ["ON", "OFF"],
            Values =
            [
                new(VersionBData.DataType.Int8, 1),
                new(VersionBData.DataType.Int8, 0)
            ]
        }
    }
];

await RunVersionBApplication();

/// <summary>
/// Runs the version B application.
/// </summary>
async Task RunVersionBApplication()
{
    var applicationOptions = new SparkplugApplicationOptions(
        "broker.hivemq.com",
        1883,
        nameof(RunVersionBApplication),
        null,
        null,
        "scada1",
        TimeSpan.FromSeconds(30),
        SparkplugMqttProtocolVersion.V311,
        null,
        null,
        true,
        CancellationTokenSource.Token);

    var application = new VersionB.SparkplugApplication(VersionBMetricsApplication, SparkplugSpecificationVersion.Version22);

    // Handles the application's connected and disconnected events.
    application.Connected += OnApplicationVersionBConnected;
    application.Disconnected += OnApplicationVersionBDisconnected;

    // Handles the application's device related events.
    application.DeviceBirthReceived += OnApplicationVersionBDeviceBirthReceived;
    application.DeviceDataReceived += OnApplicationVersionBDeviceDataReceived;
    application.DeviceDeathReceived += OnApplicationVersionBDeviceDeathReceived;

    // Handles the application's node related events.
    application.NodeBirthReceived += OnApplicationVersionBNodeBirthReceived;
    application.NodeDataReceived += OnApplicationVersionBNodeDataReceived;
    application.NodeDeathReceived += OnApplicationVersionBNodeDeathReceived;

    // Start an application.
    Console.WriteLine("Starting application...");
    await application.Start(applicationOptions);
    Console.WriteLine("Application started...");

    // Publish node commands.
    Console.WriteLine("Publishing a node command ...");
    await application.PublishNodeCommand(VersionBMetricsApplication, "group1", "edge1");

    // Publish device commands.
    Console.WriteLine("Publishing a device command ...");
    await application.PublishDeviceCommand(VersionBMetricsApplication, "group1", "edge1", "device1");

    // Get the known metrics from an application.
    var currentlyKnownMetrics = application.KnownMetrics;
    Console.WriteLine("Currently Known Metrics:");
    foreach (var metric in currentlyKnownMetrics)
    {
       Console.WriteLine($"  Metric Name: {metric.Name}, Value: {metric.Value}, Data Type: {metric.DataType}");
    }

    // Get the device states from an application.
    var currentDeviceStates = application.DeviceStates;

    // Get the node states from an application.
    var currentNodeStates = application.NodeStates;
    foreach (var currentNodeState in currentNodeStates) 
    {
        Console.WriteLine($"  Node Name: {currentNodeState.Key}, Metric State: {currentNodeState.Value.ToString}");
    }
    // Check whether an application is connected.
    var isApplicationConnected = application.IsConnected;
    Console.WriteLine("isApplicationConnected..."+ isApplicationConnected);

    // Stopping an application.
    await application.Stop();
    Console.WriteLine("Application stopped...");
}

Task OnApplicationVersionBNodeDeathReceived(SparkplugBase<Metric>.NodeDeathEventArgs args)
{
    Console.WriteLine($"Node Death Received: Group={args.GroupIdentifier}, Node={args.EdgeNodeIdentifier} ");
    return Task.CompletedTask;
}

Task OnApplicationVersionBNodeDataReceived(SparkplugApplicationBase<Metric>.NodeDataEventArgs args)
{
    Console.WriteLine($"Node Data Received: Group={args.GroupIdentifier}, Node={args.EdgeNodeIdentifier} ");
    return Task.CompletedTask;
}

Task OnApplicationVersionBNodeBirthReceived(SparkplugBase<Metric>.NodeBirthEventArgs args)
{
    Console.WriteLine($"Node Birth Received: Group={args.GroupIdentifier}, Node={args.EdgeNodeIdentifier} ");
    return Task.CompletedTask;
}

Task OnApplicationVersionBConnected(SparkplugBase<VersionBData.Metric>.SparkplugEventArgs args)
{
    Console.WriteLine("Application Connected");
    return Task.CompletedTask;
}

Task OnApplicationVersionBDisconnected(VersionB.SparkplugApplication.SparkplugEventArgs args)
{
    Console.WriteLine("Application Disconnected");
    return Task.CompletedTask;
}

Task OnApplicationVersionBDeviceBirthReceived(SparkplugBase<VersionBData.Metric>.DeviceBirthEventArgs args)
{
    Console.WriteLine($"Device Birth Received: Group={args.GroupIdentifier}, Node={args.EdgeNodeIdentifier}, Device={args.DeviceIdentifier}");
    return Task.CompletedTask;
}

/// <summary>
/// Handles the device data callback for version B applications.
/// </summary>
Task OnApplicationVersionBDeviceDataReceived(VersionB.SparkplugApplication.DeviceDataEventArgs args)
{
    Console.WriteLine($"Device Data Received: Group={args.GroupIdentifier}, Node={args.EdgeNodeIdentifier}, Device={args.DeviceIdentifier}");
    return Task.CompletedTask;
}

/// <summary>
/// Handles the device death received callback for version B applications.
/// </summary>
Task OnApplicationVersionBDeviceDeathReceived(Core.SparkplugBase<VersionBData.Metric>.DeviceEventArgs args)
{
    Console.WriteLine($"Device Death Received: Group={args.GroupIdentifier}, Node={args.EdgeNodeIdentifier}, Device={args.DeviceIdentifier}");
    return Task.CompletedTask;
}



