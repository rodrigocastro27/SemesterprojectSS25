import 'dart:convert';
import 'dart:io';
import 'dart:async';
import 'package:flutter/foundation.dart'; // For VoidCallback

import '../logic/action_dispatcher.dart';

class WebSocketService {
  final ServerActionDispatcher dispatcher;

  WebSocket? _socket;
  bool _isConnected = false;
  Completer<void>? _connectionCompleter;
  Uri? _uri; // Store the connected URI
  VoidCallback? onDisconnected; // Optional external handler
  VoidCallback? onDisconnect;
  VoidCallback? onConnect;

  void setOnDisconnect(VoidCallback callback) {
    onDisconnect = callback;
  }

  void setOnConnect(VoidCallback callback) {
    onConnect = callback;
  }

  WebSocketService(this.dispatcher);

  Future<void> connect(String url) async {
    _connectionCompleter = Completer();
    _uri = Uri.parse(url);

    try {
      _socket = await WebSocket.connect(url);
      _isConnected = true;
      onConnect?.call();

      print("✅ Connected to WebSocket at $url");

      _socket!.listen(
        _onMessageReceived,
        onDone: _onConnectionClosed,
        onError: _onConnectionError,
      );

      _connectionCompleter!.complete();
    } catch (e, stack) {
      print("❌ WebSocket connection failed: $e");
      print(stack);
      _isConnected = false;
      _connectionCompleter!.completeError(e);
    }
  }

  Future<void> get connectionDone {
    return _connectionCompleter?.future ?? Future.error("WebSocket not initialized");
  }

  void _onMessageReceived(dynamic message) {
    print("📩 Message from server: $message");

    try {
      dispatcher.handleMessage(message);
    } catch (e) {
      print("⚠️ Failed to handle message: $e");
    }
  }

  void _onConnectionClosed() {
    _isConnected = false;
    print("🔌 WebSocket connection closed");

    // Notify external listeners
    if (onDisconnected != null) {
      onDisconnected!();
    }

    onDisconnect?.call();
  }

  void _onConnectionError(error) {
    _isConnected = false;
    print("🚫 WebSocket error: $error");

    if (onDisconnected != null) {
      onDisconnected!();
    }

    onDisconnect?.call();
  }

  void send(String action, Map<String, dynamic> data) {
    if (_socket != null && _isConnected) {
      final message = jsonEncode({'action': action, 'data': data});
      _socket!.add(message);
      print("📤 Sent message: $message");
    } else {
      print("⚠️ Can't send message, socket not connected.");
    }
  }

  void disconnect() {
    _socket?.close();
    _isConnected = false;
    print("👋 Disconnected from WebSocket.");
  }

  bool get isConnected => _isConnected;

  Uri? get uri => _uri; // Optional getter if you need to know the current URL
}
