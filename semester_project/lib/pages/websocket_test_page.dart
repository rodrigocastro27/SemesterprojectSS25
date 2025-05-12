import 'package:flutter/material.dart';
import '../services/websocket_client.dart';
import 'dart:convert';

class WebSocketTestPage extends StatefulWidget {
  const WebSocketTestPage({super.key});

  @override
  State<WebSocketTestPage> createState() => _WebSocketTestPageState();
}

class _WebSocketTestPageState extends State<WebSocketTestPage> {
  late final WebSocketService socket;
  final TextEditingController _controller = TextEditingController();
  final List<String> messages = [];

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _initSocket();
    });
  }

  void _initSocket() {
    socket = WebSocketService.connect('ws://192.168.137.1:5000/ws');
    print('WebSocket: trying to connect...');

    socket.stream.listen(
      (data) {
        print('WebSocket: received $data');
        setState(() {
          messages.add('Server: $data');
        });
      },
      onError: (error) {
        print('WebSocket error: $error');
      },
    );
  }

  @override
  void dispose() {
    socket.disconnect();
    _controller.dispose();
    super.dispose();
  }

  void _sendMessage() {
    final payload = {"action": "join_lobby", "name": "Aurora", "lobbyId": "11"};
    final jsonString = jsonEncode(payload);
    socket.send(jsonString);
    setState(() {
      messages.add('Client: $jsonString');
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text("WebSocket Test Page")),
      body: Column(
        children: [
          Expanded(
            child: ListView.builder(
              itemCount: messages.length,
              itemBuilder: (_, i) => ListTile(title: Text(messages[i])),
            ),
          ),
          Padding(
            padding: const EdgeInsets.all(8.0),
            child: Row(
              children: [
                Expanded(child: TextField(controller: _controller)),
                IconButton(
                  icon: const Icon(Icons.send),
                  onPressed: _sendMessage,
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
