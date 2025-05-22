import 'package:flutter/material.dart';
import 'lobby pages/create_lobby.dart';
import 'lobby pages/join_lobby.dart';

class HomePage extends StatefulWidget{
  final String username;

  const HomePage({super.key, required this.username});

  @override
  State<HomePage> createState() => LandingPage();
}

class LandingPage extends State<HomePage>  {

 @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Multiplayer Lobby - ${widget.username}'),
        centerTitle: true,
      ),
      body: Center(
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 48),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              ElevatedButton.icon(
                onPressed: () => {
                  Navigator.push(context, MaterialPageRoute(builder: (_)=> JoinLobbyPage(username: widget.username)))
                },
                icon: const Icon(Icons.login),
                label: const Text('Join Lobby'),
                style: ElevatedButton.styleFrom(
                  minimumSize: const Size.fromHeight(50),
                  textStyle: const TextStyle(fontSize: 18),
                ),
              ),
              const SizedBox(height: 20),
              ElevatedButton.icon(
                onPressed: () => {
                  
                   Navigator.push(context, MaterialPageRoute(builder: (_) => CreateLobbyPage(username: widget.username)))
                },
                icon: const Icon(Icons.add_box),
                label: const Text('Create Lobby'),
                style: ElevatedButton.styleFrom(
                  minimumSize: const Size.fromHeight(50),
                  textStyle: const TextStyle(fontSize: 18),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}



