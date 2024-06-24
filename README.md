# SignalR Client in Flutter

This guide demonstrates how to use the `signalr_netcore` package to connect a Flutter application to a SignalR hub.
You can visit the official Markdown Guide [here](https://pub.dev/packages/signalr_netcore).

## Setup

First, add the `signalr_netcore` package to your `pubspec.yaml` file:

```yaml
dependencies:
  flutter:
    sdk: flutter
  signalr_netcore: ^0.1.0
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
import 'package:flutter/material.dart';
import 'package:signalr_netcore/signalr_client.dart';

void main() => runApp(MyApp());

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      home: SignalRExample(),
    );
  }
}

class SignalRExample extends StatefulWidget {
  @override
  _SignalRExampleState createState() => _SignalRExampleState();
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
    hubConnection = HubConnectionBuilder().withUrl(serverUrl).build();

    hubConnection.onclose((error) => print("Connection Closed"));

    hubConnection.on("ReceiveImage", _getImageFromRaspberrypi);

    await hubConnection.start();
    await hubConnection.invoke("GetImage");
  }

  void _getImageFromRaspberrypi(List<Object>? parameters) {
    if (parameters != null) {
      print("Server invoked the method with parameters: $parameters");
      final imageBytes = parameters[0] as List<int>;
      setState(() {
        _image = Image.memory(Uint8List.fromList(imageBytes));
      });
    } else {
      print("No image received");
    }
  }

  Image? _image;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("SignalR Example"),
      ),
      body: Center(
        child: _image ?? Text("No image received"),
      ),
    );
  }
}
```

This code sets up a SignalR connection, registers a method to handle incoming images, and invokes a method to request an image from the server
