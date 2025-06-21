import 'package:flutter/material.dart';
import 'package:semester_project/logic/message_sender.dart';
import 'package:uuid/uuid.dart';

class AuthPage extends StatefulWidget {
  const AuthPage ({super.key});

  @override
  State<AuthPage> createState() => _AuthPageState();
}

class _AuthPageState extends State<AuthPage> {
  final TextEditingController _usernameController = TextEditingController();

  void _login() {
    final username = _usernameController.text.trim();
    if (username.isNotEmpty) {
      
      // Use uuid for now, can use Firebase ID later
      String deviceId = Uuid().v1();

      MessageSender.loginPlayer(deviceId, username);

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Logged in as $username')),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Please enter a username')),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Player Login'),
      ),
      body: Padding(
        padding: const EdgeInsets.all(24.0),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            TextField(
              controller: _usernameController,
              decoration: InputDecoration(
                labelText: 'Username',
                border: OutlineInputBorder(),
              ),
            ),
            SizedBox(height: 20),
            ElevatedButton(
              onPressed: _login,
              style: ElevatedButton.styleFrom(
                minimumSize: Size(double.infinity, 48),
              ),
              child: Text('Log in'),
            ),
          ],
        ),
      ),
    );
  }

  @override
  void dispose() {
    _usernameController.dispose();
    super.dispose();
  }
}
