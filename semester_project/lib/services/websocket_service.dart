import 'dart:async';
import 'dart:convert';
import 'dart:io'; 

import 'package:flutter/foundation.dart';
import '../logic/action_dispatcher.dart'; 

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

  /// Connects to the WebSocket server at the specified URL.
  void connect(String url) {
    _manuallyDisconnected = false;
    _url = url; 
    _initConnection();
  }


  void _initConnection() async {
    
    _reconnectTimer?.cancel();

    try {
      print("🔌 Trying to connect to $_url...");
      _socket = await WebSocket.connect(_url);
      _isConnected = true;
      _retrySeconds = 1; 

      print("✅ Connected to WebSocket at $_url");
      _onConnectCallback?.call();

 
      _socket!.listen(
        _onMessageReceived, 
        onDone: _handleDisconnect, 
        onError: _handleError,     
      
      );
    } catch (e) {
      // This catch block handles errors during the initial WebSocket.connect() attempt (e.g., server not running)
      print("❌ WebSocket connection failed: $e");
      _isConnected = false; 
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
      print("⚠ Failed to handle message: $e");
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
    _onDisconnectCallback?.call();
 
    print("🚫 WebSocket error: $error");
    
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
      
      _retrySeconds = (_retrySeconds * 2).clamp(1, 64);
      _initConnection();
    });
  }

  /// Sends a message to the server.
  void send(String action, Map<String, dynamic> data) {
    if (_socket != null && _isConnected) {
      final message = jsonEncode({'action': action, 'data': data});
      _socket!.add(message);
      print("📤 Sent message: $message");
    } else {
     
      print("⚠ Can't send message, socket not connected. Current state: ${_socket?.readyState}");
     
    }
  }

  /// Disconnects from the WebSocket manually.
  void disconnect() {
    _manuallyDisconnected = true;
    _reconnectTimer?.cancel(); 
    _socket?.close(1000, "Client initiated disconnect"); 
    _isConnected = false;
    _onDisconnectCallback?.call(); 
    print("👋 Disconnected from WebSocket.");
  }

  bool get isConnected => _isConnected;
}