import 'package:flutter/material.dart';
import 'package:semester_project/main.dart';
import 'package:semester_project/pages/lobby%20pages/lobby_page.dart';
import 'package:semester_project/models/player.dart';
import 'package:semester_project/pages/map_page.dart';
import 'package:semester_project/services/lobby_state.dart';

import '../action_dispatcher.dart';

class LobbyActions {

  // Messages recieved from the server and how to handle them
  static void register(ServerActionDispatcher dispatcher) {
    dispatcher.register('lobby_joined', _handleLobbyJoined);
    dispatcher.register('lobby_created', _handleLobbyCreated);

    dispatcher.register('new_player_joined', _handleNewPlayerJoined);
    dispatcher.register('removed_player', _handlePlayerRemoved);

    dispatcher.register('game_started', _handleGameStarted);

    dispatcher.register('failed_lobby', _handleJoinLobbyFailed);
  }

  static void _handleLobbyJoined(Map<String, dynamic> data) {
    final lobbyId = data['lobbyId'];
    final playerData = data['player'];
    final player = Player(name: playerData['name'], role: playerData['role']);

    LobbyState.instance.setLobby(lobbyId, [player], isHost: false);

    navigatorKey.currentState?.pop();
    navigatorKey.currentState?.push(
      MaterialPageRoute(builder: (_) => const LobbyPage()),
    );
  }

  static void _handleLobbyCreated(Map<String, dynamic> data) {
    final lobbyId = data['lobbyId'];
    final playerData = data['player'];
    final player = Player(name: playerData['name'], role: playerData['role']);

    LobbyState.instance.setLobby(lobbyId, [player], isHost: true);

    navigatorKey.currentState?.pop();
    navigatorKey.currentState?.push(
      MaterialPageRoute(builder: (_) =>const LobbyPage()),
    );
  }

  static void _handleNewPlayerJoined(Map<String, dynamic> data) {
    final playerData = data['player'];
    final player = Player(name: playerData['name'], role: playerData['role']);

    LobbyState.instance.addPlayer(player);
    print('Current players in lobby:');
    for (var p in LobbyState.instance.players) {
      print('- ${p.name} (${p.role})');
    }

     navigatorKey.currentState?.push(
      MaterialPageRoute(builder: (_) =>const LobbyPage()),
    );

  }

  static void _handlePlayerRemoved(Map<String, dynamic> data) {
    final lobbyId = data['lobbyId'];
    // Optional: Show a snackbar or update UI
    print('Player removed from lobby $lobbyId');
  }

  static void _handleJoinLobbyFailed(Map<String, dynamic> data) {
    final lobbyId = data['lobbyId'];
    navigatorKey.currentState?.pop(); // remove loading if any
    showDialog(
      context: navigatorKey.currentContext!,
      builder:
          (ctx) => AlertDialog(
            title: const Text("Failed to join lobby"),
            content: Text("Could not join lobby $lobbyId because it already exists."),
            actions: [
              TextButton(
                onPressed: () => Navigator.pop(ctx),
                child: const Text("OK"),
              ),
            ],
          ),
    );
  }

  static void _handleGameStarted(Map<String, dynamic> data) {
    // Navigate to game screen or change UI state
    navigatorKey.currentState?.push(
      MaterialPageRoute(builder: (_) =>const MapPage()),
    );
  }
}
