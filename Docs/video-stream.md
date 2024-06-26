# Stream Video From Raspberrypi To Flutter

This guide explains how to implement a live video streaming feature in a Flutter application using SignalR. It includes steps for connecting to the SignalR hub, invoking methods to start and stop the live stream, and handling incoming video frames.

## Starting the Live Stream

To start the live stream, the Flutter application needs to invoke the GetLiveStream method. Here's how to do that:

```dart
// Invoke the method to start the live stream.
await hubConnection.invoke("GetLiveStream");
```

## Handling Incoming Video Frames

The Raspberry Pi will invoke the UploadLiveStream method to send video frames. You need to handle these frames in the Flutter application. Here's how to register a method to handle the incoming frames:

```dart
// Register a method to handle the incoming video frames.
hubConnection.on("ReceiveFrame", _handleIncomingFrame);

void _handleIncomingFrame(List<Object> parameters) {
  final chunk = parameters[0] as String;
  final index = parameters[1] as int;
  final totalChunks = parameters[2] as int;

  // Handle the video frame (e.g., append it to a buffer or display it)
  print("Total chunks: $totalChunks");
  print("Received frame chunk $index : $chunk");
}
```

## Stopping the Live Stream

To stop the live stream, the Flutter application needs to invoke the StopLiveStream method. Here's how to do that:

```dart
// Invoke the method to stop the live stream.
await hubConnection.invoke("StopLiveStream");
```

## Full Example

Here is a complete example of connecting to the SignalR hub, starting the live stream, handling incoming video frames, and stopping the live stream:

```dart
import 'package:flutter/material.dart';
import 'package:signalr_netcore/signalr_client.dart';

void main() => runApp(const MyApp());

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return const MaterialApp(
      home: SignalRLiveStream(),
    );
  }
}

class SignalRLiveStream extends StatefulWidget {
  const SignalRLiveStream({super.key});

  @override
  State<SignalRLiveStream> createState() => _SignalRLiveStreamState();
}

class _SignalRLiveStreamState extends State<SignalRLiveStream> {
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

      hubConnection.on("ReceiveFrame", _handleIncomingFrame);

      await hubConnection.start();
      debugPrint('Connection started');
    } on Exception catch (e) {
      debugPrint('Connection error: ${e.toString()}');
    }
  }

  void _handleIncomingFrame(List<Object> parameters) {
    final chunk = parameters[0] as String;
    final index = parameters[1] as int;
    final totalChunks = parameters[2] as int;

    // Handle the video frame (e.g., append it to a buffer or display it)
    debugPrint("Total chunks: $totalChunks");
    debugPrint("Received frame chunk $index : $chunk");
  }

  void _startLiveStream() async {
    try {
      await hubConnection.invoke("GetLiveStream");
      debugPrint('Live stream started');
    } on Exception catch (e) {
      debugPrint('Error starting live stream: ${e.toString()}');
    }
  }

  void _stopLiveStream() async {
    try {
      await hubConnection.invoke("StopLiveStream");
      debugPrint('Live stream stopped');
    } on Exception catch (e) {
      debugPrint('Error stopping live stream: ${e.toString()}');
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("SignalR Live Stream"),
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            ElevatedButton(
              onPressed: _startLiveStream,
              child: const Text("Start Live Stream"),
            ),
            const SizedBox(height: 20),
            ElevatedButton(
              onPressed: _stopLiveStream,
              child: const Text("Stop Live Stream"),
            ),
          ],
        ),
      ),
    );
  }
}
```

This code sets up a SignalR connection, registers methods to handle incoming video frames, and provides buttons to start and stop the live stream.

### Conclusion

By following this guide, you can implement a live video streaming feature in your Flutter application using SignalR. This includes setting up the SignalR connection, invoking methods to start and stop the stream, and handling incoming video frames from the server.

---

Previous: [Image stream](./images-stream.md)

Next: [Capture a single image](capture-image.md)
