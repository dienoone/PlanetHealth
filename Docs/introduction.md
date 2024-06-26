# Introduction to Connecting Flutter with Server using SignalR Core

This guide introduces how to set up a connection between a Flutter application and a server using SignalR Core. SignalR is a library for ASP.NET that simplifies the process of adding real-time web functionality to applications. It allows the server to push updates to the client, making it ideal for live chat, gaming, notifications, and other real-time features.

## Prerequisites

Before you begin, ensure you have the following:

- A Flutter development environment set up.
- A SignalR server ready to connect to.
- Basic knowledge of Flutter and Dart.

## Step-by-Step Guide

---

### Step 1: Add Dependencies

First, add the `signalr_netcore` package to your `pubspec.yaml` file:

```yaml
dependencies:
  flutter:
    sdk: flutter
  signalr_netcore: ^1.3.7
```

Run `flutter pub get` to install the package.

### Step 2: Import the SignalR Library

In your Flutter project, import the SignalR library:

```dart
import 'package:signalr_netcore/signalr_client.dart';
```

### Step 3: Define the Server URL

Specify the URL of your SignalR server. This URL should point to the SignalR hub endpoint on your server:

```dart
final serverUrl = "https://your-signalr-server-url";
```

### Step 4: Create a Hub Connection

Create an instance of `HubConnection` using the HubConnectionBuilder:

```dart
final hubConnection = HubConnectionBuilder().withUrl(serverUrl).build();
```

### Step 5: Handle Connection Events

Set up handlers for connection events such as connection closed, reconnecting, and reconnected:

```dart
hubConnection.onclose((error) => print("Connection Closed"));

hubConnection.onreconnecting((error) => print("Reconnecting: ${error?.toString()}"));

hubConnection.onreconnected((connectionId) => print("Reconnected with connection ID: $connectionId"));
```

### Step 6: Start the Connection

Start the connection to the SignalR server:

```dart
await hubConnection.start();
print('Connection started');
```

## Full Example

Here is a complete example that demonstrates how to set up a connection to a SignalR server:

```dart
import 'package:flutter/material.dart';
import 'package:signalr_netcore/signalr_client.dart';

void main() => runApp(const MyApp());

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return const MaterialApp(
      home: SignalRConnection(),
    );
  }
}

class SignalRConnection extends StatefulWidget {
  const SignalRConnection({super.key});

  @override
  State<SignalRConnection> createState() => _SignalRConnectionState();
}

class _SignalRConnectionState extends State<SignalRConnection> {
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

      hubConnection.onclose((error) {
        debugPrint('Connection closed: ${error?.toString()}');
        // Attempt to reconnect after a delay
        Future.delayed(const Duration(seconds: 5), () {
          if (hubConnection.state != HubConnectionState.Connected) {
            connectToSignalR();
          }
        });
      });

      hubConnection.onreconnecting((error) {
        debugPrint('Reconnecting: ${error?.toString()}');
      });

      hubConnection.onreconnected((connectionId) {
        debugPrint('Reconnected with connection ID: $connectionId');
      });

      await hubConnection.start();
      debugPrint('Connection started');
    } on Exception catch (e) {
      debugPrint('Connection error: ${e.toString()}');
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("SignalR Connection Example"),
      ),
      body: Center(
        child: const Text("SignalR Connection Established"),
      ),
    );
  }
}
```

By following these steps, you can establish a connection between your Flutter application and a SignalR server. This setup forms the foundation for implementing real-time features such as live chat, notifications, and data streaming.
