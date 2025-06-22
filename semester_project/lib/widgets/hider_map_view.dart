import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';
import 'package:qr_flutter/qr_flutter.dart';
import 'package:semester_project/state/game_state.dart';
import 'package:semester_project/state/lobby_state.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:semester_project/state/player_state.dart';

import 'user_marker.dart';
import '../models/ping_state.dart';

class HiderMapView extends StatelessWidget {
  const HiderMapView({super.key});

  @override
  Widget build(BuildContext context) {
    final gameState = Provider.of<GameState>(context);
    final lobbyState = Provider.of<LobbyState>(context);
    final playerState = Provider.of<PlayerState>(context);

    if (gameState.userLocation == null) {
      Provider.of<GameState>(context, listen: false).initLocation(context, lobbyState.getLobbyId()!);
      return const Center(child: CircularProgressIndicator());
    }

    return Stack(
      children: [
        FlutterMap(
          options: MapOptions(
            initialCenter: gameState.userLocation!,
            initialZoom: 16,
          ),
          children: [
            TileLayer(
              urlTemplate: 'https://tile.openstreetmap.org/{z}/{x}/{y}.png',
              userAgentPackageName: 'dev.fleaflet.flutter_map.example',
            ),
            Consumer<GameState>(
              builder: (context, gameState, _) {
                return MarkerLayer(
                  markers: gameState.pingState == PingState.pinging && gameState.userLocation != null
                      ? [
                          Marker(
                            point: gameState.userLocation!,
                            width: 28,
                            height: 28,
                            child: const UserMarker(),
                          )
                        ]
                      : [],
                );
              },
            ),
          ],
        ),
        Positioned(
          bottom: 200,
          right: 20,
          child: FloatingActionButton(
            onPressed: () {
              showDialog(
                context: context,
                builder:
                    (ctx) => AlertDialog(
                      title: const Text('Your QR Code'),
                      content: SizedBox(
                        width: 200,
                        height: 200,
                        child: Center(
                          child: QrImageView(
                            data: playerState.getPlayer()?.qrCode ?? 'unknown_id',
                            version: QrVersions.auto,
                            size: 180,
                          ),
                        ),
                      ),
                      actions: [
                        TextButton(
                          onPressed: () {
                            Navigator.of(ctx).pop(); // close dialog
                          },
                          child: const Text('Close'),
                        ),
                      ],
                    ),
              );
            },
            child: const Icon(Icons.qr_code),
          ),
        ),
      ],
    );
  }
}