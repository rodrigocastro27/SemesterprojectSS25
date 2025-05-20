import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/logic/message_sender.dart';
import 'package:semester_project/state/lobby_state.dart';
import 'package:semester_project/state/player_state.dart';
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
  final lobbyState = context.watch<LobbyState>();
  final playerState = context.watch<PlayerState>();

  return Scaffold(
    appBar: AppBar(
      title: Text('Lobby: ${lobbyState.lobbyId}'),
      actions: [
        if (lobbyState.isHost)
          const Padding(
            padding: EdgeInsets.all(8.0),
            child: Icon(Icons.star, color: Colors.amber),
          ),
      ],
    ),
    body: Column(
      children: [
        Expanded(
          child: ListView.builder(
            itemCount: lobbyState.players.length,
            itemBuilder: (context, index) =>
                PlayerListTile(player: lobbyState.players[index]),
          ),
        ),
        Padding(
          padding: const EdgeInsets.all(16.0),
          child: ElevatedButton.icon(
            onPressed: () {
              showDialog(
              context: context,
              builder: (ctx) => AlertDialog(
                title: const Text("Confirm"),
                content: Text("Are you sure you want to leave the lobby?"),
                actions: [
                  TextButton(
                    onPressed: () => Navigator.pop(ctx), // Only remove the Dialog
                    child: const Text("NO"),
                  ),
                  TextButton(
                    onPressed: () {
                      // Pop twice to remove the loading screen as well
                      final lobbyId = lobbyState.lobbyId;
                      if (lobbyId != null) {
                        MessageSender.leaveLobby(lobbyId, playerState.username!);
                      }
                      Navigator.pop(ctx);
                      },
                    child: const Text("YES"),
                  ),
        ],
      ),
    );
            },
            icon: const Icon(Icons.exit_to_app),
            label: const Text("Leave Lobby"),
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.red,
              foregroundColor: Colors.white,
            ),
          ),
        ),
      ],
    ),
    floatingActionButton: lobbyState.isHost
        ? FloatingActionButton(
            onPressed: () => _startGame(context, lobbyState),
            tooltip: 'Start Game',
            child: const Icon(Icons.play_arrow),
          )
        : null,
  );
}

}
