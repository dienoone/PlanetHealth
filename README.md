# SignalR Client in Flutter

This guide demonstrates how to use the `signalr_netcore` package to connect a Flutter application to a SignalR hub.
You can visit the official documentation guide [here](https://pub.dev/packages/signalr_netcore).

## Setup

First, add the `signalr_netcore` package to your `pubspec.yaml` file:

```yaml
dependencies:
  flutter:
    sdk: flutter
  signalr_netcore: ^1.3.7
```

## Connecting to the SignalR Hub

- To connect to the SignalR hub, use the following code:

```dart
// Import the library.
import 'package:signalr_netcore/signalr_client.dart';

// The location of the SignalR Server.
final serverUrl = "https://planethealth.azurewebsites.net/detection?type=flutter";
final hubConnection = HubConnectionBuilder().withUrl(serverUrl).build();

// When the connection is closed, print out a message to the console.
hubConnection.onclose((error) => print("Connection Closed"));

// Start the connection.
await hubConnection.start();
```

## Registering Methods

- You need to register the methods to handle responses from the server. Here's how to do that:

```dart
// Register a method to handle the response from the server.
hubConnection.on("ReceiveImage", _getImageFromRaspberrypi);

void _getImageFromRaspberrypi(List<Object>? parameters) {
  // Check if the parameters are not null.
  if (parameters != null) {
    print("Server invoked the method with parameters: $parameters");
    // This will help you to display the image.
    final imageBytes = parameters[0] as List<int>;
    Image.memory(Uint8List.fromList(imageBytes));
  } else {
    print("No image received");
  }
}
```

## Invoking Methods

- After defining all the event handlers, you can invoke methods on the server:

```dart
// Invoke a method to get an image from the server.
final result = await hubConnection.invoke("GetImage");
print("Result: '$result'");

// Invoke a method with a string parameter.
final resultWithParam = await hubConnection.invoke("MethodName", args: <Object>["ParameterValue"]);
print("Result: '$resultWithParam'");
```

## Full Example

- Here is a complete example of connecting to the SignalR hub, registering a method, and invoking a method:

```dart
import 'dart:convert';
import 'dart:typed_data';

import 'package:flutter/material.dart';
import 'package:signalr_netcore/signalr_client.dart';

void main() => runApp(const MyApp());

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return const MaterialApp(
      home: SignalRExample(),
    );
  }
}

class SignalRExample extends StatefulWidget {
  const SignalRExample({super.key});

  @override
  State<SignalRExample> createState() => _SignalRExampleState();

}

class _SignalRExampleState extends State<SignalRExample> {
  final serverUrl = "https://planethealth.azurewebsites.net/detection?type=flutter";
  late HubConnection hubConnection;

  @override
  void initState() {
    super.initState();
    connectToSignalR();
  }

  void connectToSignalR() async {
    try {
      hubConnection = HubConnectionBuilder().withUrl(serverUrl).build();

      hubConnection.onclose(({error}) {
        debugPrint('Connection closed: ${error?.toString()}');
        // Attempt to reconnect after a delay
        Future.delayed(const Duration(seconds: 5), () {
          if (hubConnection.state != HubConnectionState.Connected) {
            connectToSignalR();
          }
        });
      });

      hubConnection.onreconnecting(({error}) {
        debugPrint('Reconnecting: ${error?.toString()}');
      });

      hubConnection.onreconnected(({connectionId}) {
        debugPrint('Reconnected with connection ID: $connectionId');
      });

      hubConnection.on("ReceiveImage", _getImageFromRaspberrypi);

      await hubConnection.start();
      debugPrint('Connection started');
      await hubConnection.invoke("GetImage");
    } on Exception catch (e) {
      debugPrint('Connection error: ${e.toString()}');
    }
  }

  void _getImageFromRaspberrypi(List<Object?>? parameters) {
    if (parameters != null) {
      debugPrint("Server invoked the method with parameters: $parameters");
      final base64String = parameters[0] as String;
      final imageBytes = base64Decode(base64String);
      setState(() {
        _image = Image.memory(Uint8List.fromList(imageBytes));
      });
    } else {
      debugPrint("No image received");
    }
  }

  Image? _image;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("SignalR Example"),
      ),
      body: Center(
        child: _image ?? const Text("No image received"),
      ),
    );
  }
}
```

This code sets up a SignalR connection, registers a method to handle incoming images, and invokes a method to request an image from the server

### Another example to test

- this example is all about test and understand how the connectoin will work:

```dart
// Import the library.
import 'package:signalr_netcore/signalr_client.dart';

// The location of the SignalR Server.
final serverUrl = "https://planethealth.azurewebsites.net/detection?type=flutter";
final hubConnection = HubConnectionBuilder().withUrl(serverUrl).build();

// When the connection is closed, print out a message to the console.
hubConnection.onclose((error) => print("Connection Closed"));

// Register a method to handle the response from the server.
hubConnection.on("ReceiveMessage", _getMessageFromRaspberrypi);

void _getMessageFromRaspberrypi(List<Object> parameters) {
  // You will receive a message: "Hello from the flutter"
  print("Server invoked the method with parameters: $parameters");
}

// Start the connection.
await hubConnection.start();

// Invoke a method to send a message to the Raspberry Pi.
await hubConnection.invoke("SendMessage", args: <Object>["Hello from the flutter"]);
```

This is just a baisc example to understand more!!

### You can see the full documentation of the integration

[Full docs](Docs/readme.md)
