# MQTT Dotnet Projects

This repository contains several projects demonstrating MQTT communication using different approaches and libraries in .NET.

## Projects Overview

### 1. MQTT_Producer

A basic MQTT producer that publishes messages to an MQTT broker.

### 2. MQTT_Producer_SparkplugB_With_SparkPlugNet

An MQTT producer that uses the SparkPlugNet library to publish Sparkplug B compliant messages.

### 3. MQTT_Producer_SparkplugB_With_MQTTNet_Only

An MQTT producer that implements Sparkplug B message format using only MQTTnet, without additional Sparkplug libraries.

### 4. MQTT_Consumer_SparkplugB_With_MQTTNet_Only

An MQTT consumer that subscribes to and processes Sparkplug B messages using MQTTnet, without additional Sparkplug libraries.

## Running the Projects

### Sparkplug B Communication

To demonstrate Sparkplug B communication:

1. Run the **MQTT_Producer_SparkplugB_With_MQTTNet_Only** project as the publisher.
2. Run the **MQTT_Consumer_SparkplugB_With_MQTTNet_Only** project as the subscriber.

These two projects implement Sparkplug B compliant MQTT communication using only MQTTnet. Run them simultaneously to see the Sparkplug B messages being published and consumed.

### Basic MQTT Communication

To demonstrate basic MQTT communication without Sparkplug B:

1. Run the **MQTT_Producer** project as the publisher.
2. Run the **MQTT_Consumer** project as the subscriber.

These two projects demonstrate basic MQTT communication. Run them simultaneously to see the messages being published and consumed.

Note: Alternatively, you can use any MQTT client or broker to subscribe to the messages published by the MQTT_Producer.
### SparkPlugNet Example

To see an example using the SparkPlugNet library:

- Run the **MQTT_Producer_SparkplugB_With_SparkPlugNet** project.
- You can use the **MQTT_Consumer_SparkplugB_With_MQTTNet_Only** project or any Sparkplug B compliant MQTT client to subscribe to these messages.

## Configuration

Each project may have its own configuration for MQTT broker address, topics, and client IDs. Please check the individual project files for specific configuration details.

## Dependencies

- MQTTnet
- Google.Protobuf
- SparkplugNet (for the SparkPlugNet project only)

Ensure all NuGet packages are restored before running the projects.

## Notes

- The projects use HiveMQ's public MQTT broker by default. For production use, consider using a private MQTT broker.
- Sparkplug B is an MQTT-based protocol for Industrial IoT (IIoT) applications. The implementations here are basic examples and may not cover all aspects of the Sparkplug B specification.

## Project Relations

The relationships between the projects in this solution are illustrated in the following diagram:

![Project Relations](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/legacyofkain/MQTT_Dotnet/master/Relations.iuml)

This diagram provides a visual representation of how the different projects in this repository interact with each other and the MQTT broker.

## Configuration