import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/logic/message_sender.dart';
import 'package:semester_project/state/game_state.dart';
import 'package:semester_project/state/lobby_state.dart';
import 'package:semester_project/state/player_state.dart';
import 'package:semester_project/widgets/seeker_map_view.dart';
import 'package:semester_project/widgets/hider_map_view.dart';
import 'package:semester_project/models/player.dart';


class MapPage extends StatelessWidget {
  const MapPage({super.key});

  @override
  Widget build(BuildContext context) {
    final gameState = Provider.of<GameState>(context, listen: false);
    final lobbyState = Provider.of<LobbyState>(context, listen: false);
    final playerState = Provider.of<PlayerState>(context, listen: false);

    final mapView = gameState.isHider
        ? const HiderMapView()
        : const SeekerMapView();

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

          // Positioned button (bottom right corner, like a FAB)
          Positioned(
            bottom: 24,
            right: 24,
            child: ElevatedButton.icon(
              onPressed: () {
                    showDialog(
                      context: context,
                      builder: (ctx) => AlertDialog(
                        title: const Text("Confirm"),
                        content: const Text("Are you sure you want to leave the game? You will also leave the current lobby."),
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
                                MessageSender.leaveLobby(lobbyId, username);
                              }
                              else {
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
                padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
              ),
            ),
          ),
        ],
      ),
    );
  }
}
