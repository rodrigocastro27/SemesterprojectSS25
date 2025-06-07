import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/state/game_state.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:latlong2/latlong.dart';

import 'ping_button.dart';
import 'user_marker.dart'; // You can rename to HiderMarker if specific
import '../models/ping_state.dart';

class SeekerMapView extends StatelessWidget {
  const SeekerMapView({super.key});

  @override
  Widget build(BuildContext context) {
    final gameState = Provider.of<GameState>(context);

    // Show loading indicator while user location is initializing
    if (gameState.userLocation == null) {
      Provider.of<GameState>(context, listen: false).initLocation(context);
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

            // Only show hiders when ping is active
            MarkerLayer(
              markers: gameState.pingState == PingState.pinging
                  ? gameState.hiders
                      .where((p) => p.position != null)
                      .map(
                        (p) => Marker(
                          point: p.position!,
                          width: 28,
                          height: 28,
                          child: const UserMarker(), // Replace with HiderMarker(p) if needed
                        ),
                      )
                      .toList()
                  : [],
            ),
          ],
        ),

        // Ping Button UI
        Positioned(
          bottom: 20,
          left: 20,
          child: PingButton(
            onPing: () => gameState.startPing(context),
            pingState: gameState.pingState,
            cooldownSeconds: gameState.cooldownSeconds,
          ),
        ),
      ],
    );
  }
}
