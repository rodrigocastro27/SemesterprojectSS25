import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:semester_project/pages/lobby pages/lobby_page.dart';

class CreateLobbyPage extends StatefulWidget {
  const CreateLobbyPage({super.key});

  @override
  State<CreateLobbyPage> createState() => _CreateLobbyPageState();
}

class _CreateLobbyPageState extends State<CreateLobbyPage> {
  final TextEditingController _lobbyNameController = TextEditingController();
  String _selectedRole = 'Hider';

  void _createLobby() async {
    final lobbyName = _lobbyNameController.text.trim();
    if (lobbyName.isEmpty) return;

    final username = await _getUsername();

    try {
      final lobbyDoc = await FirebaseFirestore.instance.collection('lobbies').add({
        'lobbyName': lobbyName,
        'members': [
          {
            'name': username,
            'role': _selectedRole.toLowerCase(),
          },
        ],
      });

      Navigator.pushReplacement(
        context,
        MaterialPageRoute(builder: (_) => LobbyPage(lobbyRef: lobbyDoc)),
      );
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Failed to create lobby: $e')),
      );
    }
  }

  Future<String> _getUsername() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString('username') ??
        'Guest${DateTime.now().millisecondsSinceEpoch}';
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Color(0xffffffff),
      appBar: AppBar(
        elevation: 0,
        backgroundColor: Color(0xffffffff),
        leading: IconButton(
          icon: const Icon(Icons.arrow_back, color: Color(0xff212435)),
          onPressed: () => Navigator.pop(context),
        ),
        title: const Text(
          "Create Lobby",
          style: TextStyle(
            color: Colors.black,
            fontWeight: FontWeight.bold,
          ),
        ),
      ),
      body: SingleChildScrollView(
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 16),
          child: Column(
            children: [
              ClipRRect(
                borderRadius: BorderRadius.circular(16),
                child: Image.asset(
                  'assets/images/floppa.png',
                  height: 120,
                  width: 120,
                  fit: BoxFit.contain,
                ),
              ),
              const SizedBox(height: 20),
              
              TextField(
                controller: _lobbyNameController,
                decoration: InputDecoration(
                  labelText: "Lobby Name",
                  labelStyle: TextStyle(color: Color(0xff7c7878)),
                  enabledBorder: UnderlineInputBorder(
                    borderSide: BorderSide(color: Color(0xff0c9c90)),
                  ),
                  focusedBorder: UnderlineInputBorder(
                    borderSide: BorderSide(color: Color(0xff0c9c90)),
                  ),
                ),
              ),
              const SizedBox(height: 20),
              
              DropdownButtonFormField<String>(
                value: _selectedRole,
                decoration: InputDecoration(
                  labelText: "Your Role",
                  labelStyle: TextStyle(color: Color(0xff7c7878)),
                  enabledBorder: UnderlineInputBorder(
                    borderSide: BorderSide(color: Color(0xff0c9c90)),
                  ),
                  focusedBorder: UnderlineInputBorder(
                    borderSide: BorderSide(color: Color(0xff0c9c90)),
                  ),
                ),
                items: ['Hider', 'Seeker']
                    .map((role) => DropdownMenuItem(
                          value: role,
                          child: Text(role),
                        ))
                    .toList(),
                onChanged: (value) {
                  setState(() {
                    _selectedRole = value!;
                  });
                },
              ),
              const SizedBox(height: 40),
              
              MaterialButton(
                onPressed: _createLobby,
                color: Color(0xff36c8bb),
                elevation: 0,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(22.0),
                ),
                padding: const EdgeInsets.all(16),
                child: const Text(
                  "Create Lobby",
                  style: TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.w700,
                    color: Colors.white,
                  ),
                ),
                height: 50,
                minWidth: MediaQuery.of(context).size.width,
              ),
            ],
          ),
        ),
      ),
    );
  }
}