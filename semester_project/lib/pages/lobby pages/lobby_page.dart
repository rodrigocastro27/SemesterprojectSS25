import 'package:flutter/material.dart';
import 'package:semester_project/logic/message_sender.dart';
import 'package:semester_project/services/lobby_state.dart';
import 'package:semester_project/widgets/player_list_tile.dart';

class LobbyPage extends StatefulWidget {
  const LobbyPage({super.key});

  @override
  State<LobbyPage> createState() => _LobbyPageState();
}

class _LobbyPageState extends State<LobbyPage> {
  final state = LobbyState.instance;

  void _startGame() {
    final lobbyId = state.lobbyId;
    if (lobbyId == null) return;

    // Send "start_game" message to server
    MessageSender.startGame(lobbyId);

    print('Start game message sent for lobby $lobbyId');
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Lobby: ${state.lobbyId}'),
        actions: [
          if (state.isHost)
            const Padding(
              padding: EdgeInsets.all(8.0),
              child: Icon(Icons.star, color: Colors.amber),
            )
        ],
      ),
      body: ListView.builder(
        itemCount: state.players.length,
        itemBuilder: (context, index) =>
            PlayerListTile(player: state.players[index]),
      ),
      floatingActionButton: state.isHost
          ? FloatingActionButton(
              onPressed: _startGame,
              child: const Icon(Icons.play_arrow),
              tooltip: 'Start Game',
            )
          : null,
    );
  }
}
