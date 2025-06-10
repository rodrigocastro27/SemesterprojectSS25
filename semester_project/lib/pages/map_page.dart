import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/logic/message_sender.dart';
import 'package:semester_project/state/game_state.dart';
import 'package:semester_project/state/lobby_state.dart';
import 'package:semester_project/state/player_state.dart';
import 'package:semester_project/widgets/seeker_map_view.dart';
import 'package:semester_project/widgets/hider_map_view.dart';
import 'package:semester_project/widgets/task_overlay.dart';

class MapPage extends StatelessWidget {
  const MapPage({super.key});

  @override
  Widget build(BuildContext context) {
    final gameState = Provider.of<GameState>(context, listen: false);
    final lobbyState = Provider.of<LobbyState>(context, listen: false);
    final playerState = Provider.of<PlayerState>(context, listen: false);

    final mapView =
        gameState.isHider ? const HiderMapView() : const SeekerMapView();

    return Scaffold(
      appBar: AppBar(
        title: const Text('Map'),
        backgroundColor: Colors.blueGrey,
        foregroundColor: Colors.white,
        shadowColor: Colors.black,
      ),
      body: Stack(
        children: [
          // Fullscreen map view
          Positioned.fill(child: mapView),

          //Countdown timer
          Positioned(
            top: 16,
            left: 16,
            child: Consumer<GameState>(
              builder: (context, gameState, child) {
                final remaining = gameState.remainingTime;
                if (remaining == null) return const SizedBox();

                String format(Duration d) =>
                    "${d.inMinutes.remainder(60).toString().padLeft(2, '0')}:${(d.inSeconds.remainder(60)).toString().padLeft(2, '0')}";

                return Container(
                  padding: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    color: Colors.black.withOpacity(0.7),
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Text(
                    "Time left: ${format(remaining)}",
                    style: const TextStyle(color: Colors.white, fontSize: 18),
                  ),
                );
              },
            ),
          ),

          Positioned(
            top: 24,
            right: 24,
            child: Align(
              alignment: Alignment.center,
              child: Consumer<GameState>(
                builder: (context, gameState, _) {
                  final isTaskOngoing = gameState.currentTaskName != null;

                  return ElevatedButton.icon(
                    onPressed: isTaskOngoing
                        ? null // Disabled when task is ongoing
                        : () {
                            MessageSender.startTask(
                              playerState.getUsername()!,
                              lobbyState.getLobbyId()!,
                            );
                          },
                    icon: const Icon(Icons.task),
                    label: const Text("Start Task"),
                    style: ElevatedButton.styleFrom(
                      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
                    ),
                  );
                },
              ),
            ),
          ),

          Consumer<GameState>(
            builder: (context, gameState, _) {
              if (gameState.currentTaskName != null) return const TaskOverlay();
              return Container();   // to not build the correct widget
            },
          ),

          // Positioned button (bottom right corner, like a FAB)
          Positioned(
            bottom: 24,
            right: 24,
            child: ElevatedButton.icon(
              onPressed: () {
                showDialog(
                  context: context,
                  builder:
                      (ctx) => AlertDialog(
                        title: const Text("Confirm"),
                        content: const Text(
                          "Are you sure you want to leave the game? You will also leave the current lobby.",
                        ),
                        actions: [
                          TextButton(
                            onPressed: () => Navigator.pop(ctx),
                            child: const Text("NO"),
                          ),
                          TextButton(
                            onPressed: () {
                              final username = playerState.getUsername();
                              final lobbyId = lobbyState.lobbyId;
                              if (username != null && lobbyId != null) {
                                lobbyState.stopPlaying();
                                gameState.stopGame();
                                MessageSender.leaveLobby(lobbyId, username);
                              } else {
                                // Should never happen
                              }
                            },
                            child: const Text("YES"),
                          ),
                        ],
                      ),
                );
              },
              icon: const Icon(Icons.logout),
              label: const Text("Leave Game"),
              style: ElevatedButton.styleFrom(
                padding: const EdgeInsets.symmetric(
                  horizontal: 16,
                  vertical: 12,
                ),
              ),
            ),
          ),

          Consumer<GameState>(
            builder: (context, gameState, _) {
              final result = gameState.taskResult;

              if (result == null) return const SizedBox();

              String message;
              Color color;

              switch (result) {
                case 'hiders':
                  message = 'Hiders Win!';
                  color = Colors.green;
                  break;
                case 'seekers':
                  message = 'Seekers Win!';
                  color = Colors.red;
                  break;
                case 'tie':
                  message = 'It\'s a Tie!';
                  color = Colors.orange;
                  break;
                default:
                  return const SizedBox();
              }

              return Positioned.fill(
                child: Container(
                  color: Colors.black.withOpacity(0.7),
                  alignment: Alignment.center,
                  child: Container(
                    padding: const EdgeInsets.all(24),
                    decoration: BoxDecoration(
                      color: color,
                      borderRadius: BorderRadius.circular(20),
                      boxShadow: [
                        BoxShadow(color: Colors.black45, blurRadius: 12),
                      ],
                    ),
                    child: Text(
                      message,
                      style: const TextStyle(
                        fontSize: 32,
                        fontWeight: FontWeight.bold,
                        color: Colors.white,
                      ),
                    ),
                  ),
                ),
              );
            },
          ),
        ],
      ),
    );
  }
}
