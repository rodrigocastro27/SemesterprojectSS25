import 'package:flutter/material.dart';
import 'package:semester_project/logic/message_sender.dart';
import 'package:semester_project/models/player.dart';

class JoinLobbyPage extends StatefulWidget {
  const JoinLobbyPage({super.key});

  @override
  State<JoinLobbyPage> createState() => _JoinLobbyPageState();
}

class _JoinLobbyPageState extends State<JoinLobbyPage> {
  final TextEditingController _lobbyNameController = TextEditingController();
  final TextEditingController _usernameController = TextEditingController();
  String _selectedRole = 'Hider';

  void _joinLobby() async {
    final lobbyName = _lobbyNameController.text.trim();
    final username = _usernameController.text.trim();
    if (lobbyName.isEmpty || username.isEmpty) return;

    final player = Player(name: username, role: _selectedRole);

    MessageSender.joinLobby(lobbyName, username);

    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (_) => const Center(child: CircularProgressIndicator()),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text("Join Lobby")),
      body: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          children: [
            TextField(
              controller: _lobbyNameController,
              decoration: const InputDecoration(labelText: "Lobby Name"),
            ),
            const SizedBox(height: 20),
            TextField(
              controller: _usernameController,
              decoration: const InputDecoration(labelText: "Username"),
            ),
            const SizedBox(height: 20),
            DropdownButton<String>(
              value: _selectedRole,
              onChanged: (value) {
                setState(() {
                  _selectedRole = value!;
                });
              },
              items:
                  ['Hider', 'Seeker']
                      .map(
                        (role) =>
                            DropdownMenuItem(value: role, child: Text(role)),
                      )
                      .toList(),
            ),
            const SizedBox(height: 20),
            ElevatedButton(
              onPressed: _joinLobby,
              child: const Text("Join Lobby"),
            ),
          ],
        ),
      ),
    );
  }
}
