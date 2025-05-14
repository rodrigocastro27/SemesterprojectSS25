import 'package:flutter/material.dart';

class LobbyPage extends StatelessWidget {
  final String lobbyId;

  const LobbyPage({super.key, required this.lobbyId});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Lobby')),
      body: Center(
        child: Text(
          'Lobby: $lobbyId',  // Display the lobby ID as the name
          style: const TextStyle(fontSize: 24),
        ),
      ),
    );
  }
}
