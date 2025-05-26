import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/state/game_state.dart';
import 'package:flutter_map/flutter_map.dart';

import 'ping_button.dart';
import 'user_marker.dart';
import '../models/ping_state.dart';

class SeekerMapView extends StatelessWidget {
  const SeekerMapView({super.key});

  @override
  Widget build(BuildContext context) {
    final gameState = Provider.of<GameState>(context);

    if (gameState.userLocation == null) {
      Provider.of<GameState>(context).initLocation(context);
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
            MarkerLayer(
              markers: gameState.pingState == PingState.pinging
                  ? [Marker(point: gameState.userLocation!, width: 28, height: 28, child: const UserMarker())]
                  : [],
            ),
          ],
        ),
        Positioned(
          bottom: 20,
          left: 20,
          child: PingButton(
            onPing: () => gameState.startPing(() {}),
            pingState: gameState.pingState,
            cooldownSeconds: gameState.cooldownSeconds,
          ),
        ),
      ],
    );
  }  
}
