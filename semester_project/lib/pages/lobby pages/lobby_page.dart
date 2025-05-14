import 'package:flutter/material.dart';
import 'package:semester_project/services/lobby_state.dart';
import 'package:semester_project/widgets/player_list_tile.dart';

class LobbyPage extends StatelessWidget {
  const LobbyPage({super.key});

  @override
  Widget build(BuildContext context) {
    final state = LobbyState.instance;

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
    );
  }
}
