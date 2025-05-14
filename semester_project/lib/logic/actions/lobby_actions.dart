import 'package:flutter/material.dart';
import 'package:semester_project/main.dart';
import 'package:semester_project/pages/lobby%20pages/lobby_page.dart';
import 'package:semester_project/models/player.dart';
import 'package:semester_project/services/lobby_state.dart';

import '../action_dispatcher.dart';

class LobbyActions {
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
    final playersData = data['players'] ?? [];
    final players = (playersData as List)
        .map((p) => Player(name: p['name'], role: p['role']))
        .toList();

    LobbyState.instance.setLobby(lobbyId, players, isHost: false);

    navigatorKey.currentState?.pop();
    navigatorKey.currentState?.push(MaterialPageRoute(builder: (_) => const LobbyPage()));
  }

  static void _handleLobbyCreated(Map<String, dynamic> data) {
    final lobbyId = data['lobbyId'];
    final playersData = data['players'] ?? [];
    final players = (playersData as List)
        .map((p) => Player(name: p['name'], role: p['role']))
        .toList();

    LobbyState.instance.setLobby(lobbyId, players, isHost: true);

    navigatorKey.currentState?.pop();
    navigatorKey.currentState?.push(MaterialPageRoute(builder: (_) => const LobbyPage()));
  }

  static void _handleNewPlayerJoined(Map<String, dynamic> data) {
    final playersData = data['players'] as List;
    final players = playersData
        .map((p) => Player(name: p['name'], role: p['role']))
        .toList();

    LobbyState.instance.updatePlayers(players);
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
      builder: (ctx) => AlertDialog(
        title: const Text("Join Failed"),
        content: Text("Could not join lobby $lobbyId"),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx), child: const Text("OK"))
        ],
      ),
    );
  }

  static void _handleGameStarted(Map<String, dynamic> data) {
    // Navigate to game screen or change UI state
    print("Game has started!");
  }
}
