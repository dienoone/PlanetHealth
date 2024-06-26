# Capture Image From Raspberrypi To Flutter

This guide explains how to implement a single image capture feature in a Flutter application using SignalR. It includes steps for invoking the method to capture a single image, and handling the incoming image.

## Capturing a Single Image

To capture a single image, the Flutter application needs to invoke the `CaptureImage` method. Here's how to do that:

```dart
// Invoke the method to capture a single image.
await hubConnection.invoke("CaptureImage");
```

## Handling Incoming Image

The Raspberry Pi will send the captured image to the `Take` method. You need to handle this image in the Flutter application. Here's how to register a method to handle the incoming image:

```dart
// Register a method to handle the incoming image.
hubConnection.on("Take", _handleIncomingImage);

void _handleIncomingImage(List<Object> parameters) {
  final fileBytes = parameters[0] as List<int>;
  // Handle the image (e.g., display it)
  print("Received image with bytes: $fileBytes");
}
```

## Full Example

Here is a complete example of connecting to the SignalR hub, capturing a single image, and handling the incoming image:

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
      home: SignalRImageCapture(),
    );
  }
}

class SignalRImageCapture extends StatefulWidget {
  const SignalRImageCapture({super.key});

  @override
  State<SignalRImageCapture> createState() => _SignalRImageCaptureState();
}

class _SignalRImageCaptureState extends State<SignalRImageCapture> {
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

      hubConnection.on("Take", _handleIncomingImage);

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

  void _captureImage() async {
    try {
      await hubConnection.invoke("CaptureImage");
      debugPrint('Capture image invoked');
    } on Exception catch (e) {
      debugPrint('Error capturing image: ${e.toString()}');
    }
  }

  Image? _image;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("SignalR Single Image Capture"),
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            ElevatedButton(
              onPressed: _captureImage,
              child: const Text("Capture Image"),
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

## Important Note

If the Flutter application is using the live video stream feature, invoking the `CaptureImage` method will temporarily stop the live stream to capture the image and then send it back to the server. After capturing the image, the live stream will continue.

### Conclusion

By following this guide, you can implement a single image capture feature in your Flutter application using SignalR. This includes setting up the SignalR connection, invoking the method to capture an image, and handling the incoming image from the server.

---

Previous: [Video stream](./video-stream.md)
