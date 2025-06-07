import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/state/game_state.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:semester_project/state/player_state.dart';

import 'ping_button.dart';
import 'user_marker.dart';
import '../models/ping_state.dart';

class SeekerMapView extends StatelessWidget {
  const SeekerMapView({super.key});

  @override
  Widget build(BuildContext context) {
    final gameState = Provider.of<GameState>(context);
    final playerState = Provider.of<PlayerState>(context);

    if (gameState.userLocation == null) {
      Provider.of<GameState>(context).initLocation(context);
      return const Center(child: CircularProgressIndicator());
    }

    // Get the current player's ID
    final currentPlayerId = playerState.getDeviceId();
    
    // Check if there's an active ping
    final bool isPingActive = gameState.currentPingingSeekerId != null;
    final bool isCurrentPlayerPinging = gameState.currentPingingSeekerId == currentPlayerId;
    
    // If this player initiated the ping, update the ping state
    if (isCurrentPlayerPinging && gameState.pingState == PingState.cooldown) {
      gameState.pingState = PingState.pinging;
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
              markers: [
                // Show the current seeker's marker
                Marker(
                  point: gameState.userLocation!,
                  width: 28,
                  height: 28,
                  child: UserMarker(
                    isPinging: isCurrentPlayerPinging,
                  ),
                ),
                
                // Show other seekers' markers
                ...gameState.seekers
                    .where((seeker) => 
                        seeker.name != playerState.getUsername() && 
                        seeker.position != null)
                    .map((seeker) => Marker(
                          point: seeker.position!,
                          width: 28,
                          height: 28,
                          child: UserMarker(
                            isPinging: seeker.name == gameState.currentPingingSeekerId,
                            isOtherSeeker: true,
                          ),
                        )),
                
                // Show hiders' locations if there's an active ping
                if (isPingActive)
                  ...gameState.hiders
                      .where((hider) => hider.position != null)
                      .map((hider) => Marker(
                            point: hider.position!,
                            width: 28,
                            height: 28,
                            child: const UserMarker(isHider: true),
                          )),
              ],
            ),
          ],
        ),
        Positioned(
          bottom: 20,
          left: 20,
          child: PingButton(
            onPing: () => gameState.startPing(context),
            pingState: gameState.pingState,
            cooldownSeconds: gameState.cooldownSeconds,
          ),
        ),
        
        // Show rejection message if ping was rejected
        if (gameState.isPingRejected)
          Positioned(
            top: 20,
            left: 0,
            right: 0,
            child: Container(
              padding: const EdgeInsets.all(8),
              color: Colors.red.withOpacity(0.8),
              child: Text(
                "Ping rejected: ${gameState.pingRejectionReason}",
                textAlign: TextAlign.center,
                style: const TextStyle(color: Colors.white),
              ),
            ),
          ),
      ],
    );
  }  
}

