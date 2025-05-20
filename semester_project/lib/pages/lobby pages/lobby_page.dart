import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/logic/message_sender.dart';
import 'package:semester_project/state/lobby_state.dart';
import 'package:semester_project/widgets/player_list_tile.dart';

class LobbyPage extends StatelessWidget {
  const LobbyPage({super.key});

  void _startGame(BuildContext context, LobbyState state) {
    final lobbyId = state.lobbyId;
    if (lobbyId == null) return;

    MessageSender.startGame(lobbyId);
    print('Start game message sent for lobby $lobbyId');
  }

  @override
  Widget build(BuildContext context) {
    final state = context.watch<LobbyState>();

    return Scaffold(
      appBar: AppBar(
        title: Text('Lobby: ${state.lobbyId}'),
        actions: [
          if (state.isHost)
            const Padding(
              padding: EdgeInsets.all(8.0),
              child: Icon(Icons.star, color: Colors.amber),
            ),
        ],
      ),
      body: ListView.builder(
        itemCount: state.players.length,
        itemBuilder: (context, index) =>
            PlayerListTile(player: state.players[index]),
      ),
      floatingActionButton: state.isHost
          ? FloatingActionButton(
              onPressed: () => _startGame(context, state),
              child: const Icon(Icons.play_arrow),
              tooltip: 'Start Game',
            )
          : null,
    );
  }
}
