import 'dart:math';

import 'package:flutter/material.dart';
import 'package:semester_project/logic/message_sender.dart';

class CreateLobbyPage extends StatefulWidget {
  final String username;
  const CreateLobbyPage({super.key, required this.username});

  @override
  State<CreateLobbyPage> createState() => _CreateLobbyPageState();
}

class _CreateLobbyPageState extends State<CreateLobbyPage> {
  final TextEditingController _lobbyNameController = TextEditingController();

  void _createLobby() async {
    final lobbyName = _lobbyNameController.text.trim();
    if (lobbyName.isEmpty) return;

    MessageSender.createLobby(lobbyName, widget.username);

    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (_) => const Center(child: CircularProgressIndicator()),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text("Create Lobby")),
      body: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          children: [
            TextField(
              controller: _lobbyNameController,
              decoration: const InputDecoration(labelText: "Lobby Name"),
            ),
            const SizedBox(height: 20),
            ElevatedButton(
              onPressed: _createLobby,
              child: const Text("Create"),
            ),
          ],
        ),
      ),
    );
  }
}
