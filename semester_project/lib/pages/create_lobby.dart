import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';

class CreateLobbyPage extends StatefulWidget {
  const CreateLobbyPage({super.key});

  @override
  State<CreateLobbyPage> createState() => _CreateLobbyPageState();
}

class _CreateLobbyPageState extends State<CreateLobbyPage> {
  final TextEditingController _lobbyNameController = TextEditingController();

  void _createLobby() async {
    final lobbyName = _lobbyNameController.text.trim();
    if (lobbyName.isEmpty) return;

    final username = await _getUsername();

    await FirebaseFirestore.instance.collection('lobbies').add({
      'lobbyName': lobbyName,
      'members': [username],
    });

    Navigator.pop(context);
  }

  Future<String> _getUsername() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString('username') ?? 'Guest${DateTime.now().millisecondsSinceEpoch}';
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
