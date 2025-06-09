// lib/services/websocket_service.dart
import 'dart:async';
import 'dart:convert';
import 'dart:io'; // Required for WebSocket

import 'package:flutter/foundation.dart'; // Required for VoidCallback
import '../logic/action_dispatcher.dart'; // Assuming your dispatcher path

class WebSocketService {
  final ServerActionDispatcher dispatcher;
  WebSocket? _socket;

  bool _isConnected = false;
  bool _manuallyDisconnected = false;
  int _retrySeconds = 1;
  Timer? _reconnectTimer;

  // The URL is now set by the connect() method, so it's not final
  String _url = ''; 

  // Callbacks for connection status
  VoidCallback? _onConnectCallback;
  VoidCallback? _onDisconnectCallback;

  // Constructor now only takes the dispatcher; URL is passed to connect()
  WebSocketService(this.dispatcher); 

  /// Sets the callback to be invoked when the WebSocket successfully connects.
  void setOnConnect(VoidCallback callback) {
    _onConnectCallback = callback;
  }

  /// Sets the callback to be invoked when the WebSocket disconnects (gracefully or due to error).
  void setOnDisconnect(VoidCallback callback) {
    _onDisconnectCallback = callback;
  }

  /// Initiates the WebSocket connection to the provided URL.
  void connect(String url) {
    _manuallyDisconnected = false;
    _url = url; // Store the URL for reconnects
    _initConnection();
  }

  /// Internal method to handle connection attempts and stream listening.
  void _initConnection() async {
    // Cancel any existing reconnect timer if we're trying to connect now
    _reconnectTimer?.cancel();

    try {
      print("🔌 Trying to connect to $_url...");
      _socket = await WebSocket.connect(_url);
      _isConnected = true;
      _retrySeconds = 1; // Reset retry seconds on successful connection

      print("✅ Connected to WebSocket at $_url");
      _onConnectCallback?.call(); // Invoke the onConnect callback

      // Listen to the WebSocket stream
      _socket!.listen(
        _onMessageReceived, // <--- This is the method that was causing the "Undefined name" error
        onDone: _handleDisconnect, // Called when connection is closed (gracefully)
        onError: _handleError,     // Called when an error occurs on the socket (e.g., abrupt disconnect)
        // Removed: cancelOnError: true, to ensure both onDone/onError are properly handled
      );
    } catch (e) {
      // This catch block handles errors during the initial WebSocket.connect() attempt (e.g., server not running)
      print("❌ WebSocket connection failed: $e");
      _isConnected = false; // Ensure connection state is false
      _scheduleReconnect();
    }
  }

  /// Handles incoming messages from the server.
  void _onMessageReceived(dynamic message) {
    print("📩 Message from server: $message");

    try {
      // Delegate message handling to the dispatcher
      dispatcher.handleMessage(message);
    } catch (e) {
      print("⚠️ Failed to handle message: $e");
    }
  }

  /// Handles the WebSocket connection being closed gracefully or abruptly.
  void _handleDisconnect() {
    _isConnected = false;
    _onDisconnectCallback?.call(); // Invoke the onDisconnect callback
    print("🔌 WebSocket connection closed.");
    // Only attempt to reconnect if not manually disconnected
    if (!_manuallyDisconnected) {
      _scheduleReconnect();
    } else {
      print("👋 Manually disconnected, no reconnect attempt.");
    }
  }

  /// Handles errors occurring on the active WebSocket connection.
  void _handleError(error) {
    _isConnected = false;
    _onDisconnectCallback?.call(); // Invoke the onDisconnect callback
    // Log the actual error object for better debugging
    print("🚫 WebSocket error: $error");
    // Only attempt to reconnect if not manually disconnected
    if (!_manuallyDisconnected) {
      _scheduleReconnect();
    } else {
      print("👋 Manually disconnected due to error, no reconnect attempt.");
    }
  }

  /// Schedules a reconnect attempt with exponential backoff.
  void _scheduleReconnect() {
    // Prevent multiple reconnect timers from running concurrently
    if (_reconnectTimer != null && _reconnectTimer!.isActive) {
      return;
    }

    final delay = Duration(seconds: _retrySeconds);
    print("⏳ Reconnecting in ${_retrySeconds}s...");

    _reconnectTimer = Timer(delay, () {
      // Exponential backoff, clamp to prevent extremely long delays
      _retrySeconds = (_retrySeconds * 2).clamp(1, 64);
      _initConnection(); // Attempt to reconnect using the stored _url
    });
  }

  /// Sends a message to the server.
  void send(String action, Map<String, dynamic> data) {
    if (_socket != null && _isConnected) {
      final message = jsonEncode({'action': action, 'data': data});
      _socket!.add(message);
      print("📤 Sent message: $message");
    } else {
      // Added more detail to the warning for debugging
      print("⚠️ Can't send message, socket not connected. Current state: ${_socket?.readyState}");
      // Optionally, you might want to queue messages or throw an error here.
    }
  }

  /// Disconnects from the WebSocket manually.
  void disconnect() {
    _manuallyDisconnected = true;
    _reconnectTimer?.cancel(); // Cancel any pending reconnects
    _socket?.close(1000, "Client initiated disconnect"); // 1000 is normal closure status code
    _isConnected = false;
    _onDisconnectCallback?.call(); // Invoke the onDisconnect callback on manual disconnect
    print("👋 Disconnected from WebSocket.");
  }

  bool get isConnected => _isConnected;
}