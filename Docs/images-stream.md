# Stream Images From Raspberrypi To Flutter

This guide explains how to implement an image streaming feature in a Flutter application using SignalR. It includes steps for invoking methods to start and stop the image stream, and handling incoming images.

## Starting the Image Stream

To start the image stream, the Flutter application needs to invoke the `InitCapture` method. Here's how to do that:

```dart
// Invoke the method to start the image stream with a time interval (e.g., 2 seconds).
// 2 seconds means that raspberry pi will send an image every 2 seconds.
await hubConnection.invoke("InitCapture", args: [2]);
```

## Handling Incoming Images

The Raspberry Pi will send images to the `Capture` method. You need to handle these images in the Flutter application. Here's how to register a method to handle the incoming images:

```dart
// Register a method to handle the incoming images.
hubConnection.on("Capture", _handleIncomingImage);

void _handleIncomingImage(List<Object> parameters) {
  final fileBytes = parameters[0] as List<int>;
  // Handle the image (e.g., display it)
  print("Received image with bytes: $fileBytes");
}
```

## Stopping the Image Stream

To stop the image stream, the Flutter application needs to invoke the `EndCapture` method. Here's how to do that, Note you need to replace the `serverUrl` with the actual url:

```dart
// Invoke the method to stop the image stream.
await hubConnection.invoke("EndCapture");
```

## Full Example

Here is a complete example of connecting to the SignalR hub, starting the image stream, handling incoming images, and stopping the image stream:

```dart
import 'dart:typed_data';
import 'package:flutter/material.dart';
import 'package:signalr_netcore/signalr_client.dart';

void main() => runApp(const MyApp());

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return const MaterialApp(
      home: SignalRImageStream(),
    );
  }
}

class SignalRImageStream extends StatefulWidget {
  const SignalRImageStream({super.key});

  @override
  State<SignalRImageStream> createState() => _SignalRImageStreamState();
}

class _SignalRImageStreamState extends State<SignalRImageStream> {
  final serverUrl = "https://your-signalr-server-url";
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

      hubConnection.on("Capture", _handleIncomingImage);

      await hubConnection.start();
      debugPrint('Connection started');
    } on Exception catch (e) {
      debugPrint('Connection error: ${e.toString()}');
    }
  }

  void _handleIncomingImage(List<Object> parameters) {
    // Handle the image (e.g., display it)
    debugPrint("Received image with bytes: $fileBytes");
    final fileBytes = parameters[0] as List<int>;
    final base64String = parameters[0] as String;
    final imageBytes = base64Decode(base64String);

    setState(() {
      _image = Image.memory(Uint8List.fromList(imageBytes));
    });
  }

  void _startImageStream(int interval) async {
    try {
      await hubConnection.invoke("InitCapture", args: [interval]);
      debugPrint('Image stream started');
    } on Exception catch (e) {
      debugPrint('Error starting image stream: ${e.toString()}');
    }
  }

  void _stopImageStream() async {
    try {
      await hubConnection.invoke("EndCapture");
      debugPrint('Image stream stopped');
    } on Exception catch (e) {
      debugPrint('Error stopping image stream: ${e.toString()}');
    }
  }

  Image? _image;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("SignalR Image Stream"),
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            ElevatedButton(
              onPressed: () => _startImageStream(2),
              child: const Text("Start Image Stream"),
            ),
            const SizedBox(height: 20),
            ElevatedButton(
              onPressed: _stopImageStream,
              child: const Text("Stop Image Stream"),
            ),
            const SizedBox(height: 20),
            _image ?? const Text("No image received"),
          ],
        ),
      ),
    );
  }
}
```

This code sets up a SignalR connection, registers methods to handle incoming images, and provides buttons to start and stop the image stream.

### Conclusion

By following this guide, you can implement an image streaming feature in your Flutter application using SignalR. This includes setting up the SignalR connection, invoking methods to start and stop the stream, and handling incoming images from the server.
