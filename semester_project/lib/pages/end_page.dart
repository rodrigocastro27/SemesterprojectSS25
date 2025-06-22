import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/logic/message_sender.dart';
import 'package:semester_project/state/lobby_state.dart';
import 'package:semester_project/state/player_state.dart';
import 'package:semester_project/state/game_state.dart'; // Import GameState

class EndGamePage extends StatelessWidget {
  final String resultMessage;
  final String? additionalInfo;

  const EndGamePage({
    super.key,
    required this.resultMessage,
    this.additionalInfo,
  });

  @override
  Widget build(BuildContext context) {
    final lobbyState = Provider.of<LobbyState>(context, listen: false);
    final playerState = Provider.of<PlayerState>(context, listen: false);
    final gameState = Provider.of<GameState>(context, listen: false);

    final winner = gameState.getWinners();

    return Scaffold(
      appBar: AppBar(title: const Text("Game Over")),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(
              resultMessage,
              style: const TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 12),
            Text(
              "Winner: $winner",
              style: const TextStyle(fontSize: 18),
              textAlign: TextAlign.center,
            ),
            if (additionalInfo != null) ...[
              const SizedBox(height: 12),
              Text(additionalInfo!),
            ],
            const SizedBox(height: 24),
            ElevatedButton(
              onPressed: () {
                final username = playerState.getUsername();
                final lobbyId = lobbyState.lobbyId;
                if (username != null && lobbyId != null) {
                  lobbyState.stopPlaying(); // playing = false
                  MessageSender.leaveLobby(lobbyId, username);
                }
              },
              child: const Text("Return to Home"),
            ),
          ],
        ),
      ),
    );
  }
}
