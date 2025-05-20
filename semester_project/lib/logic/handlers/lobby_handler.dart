import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/models/player.dart';
import 'package:semester_project/state/lobby_state.dart';
import '../action_dispatcher.dart';
import 'package:semester_project/services/navigation_service.dart';

class LobbyActions {
  static void register(ServerActionDispatcher dispatcher, BuildContext context) {
    dispatcher.register('lobby_joined', (data) {
      final lobbyId = data['lobbyId'];
      final playerData = data['player'];
      final player = Player(name: playerData['name'], role: playerData['role']);
      Provider.of<LobbyState>(context, listen: false).setLobby(lobbyId, [player], isHost: false);
    });

    dispatcher.register('lobby_created', (data) {
      final lobbyId = data['lobbyId'];
      final playerData = data['player'];
      final player = Player(name: playerData['name'], role: playerData['role']);
      Provider.of<LobbyState>(context, listen: false).setLobby(lobbyId, [player], isHost: true);
    });

    dispatcher.register('new_player_joined', (data) {
      final playerData = data['player'];
      final player = Player(name: playerData['name'], role: playerData['role']);
      Provider.of<LobbyState>(context, listen: false).addPlayer(player);
    });

    dispatcher.register('game_started', (data) {
      // Handle game start, optionally update state to trigger redirect
    });

    dispatcher.register('failed_lobby', (data) {
      _showError("Could not join lobby because it already exists.");
    });

    dispatcher.register('leave_lobby', (data) {
      final playerData = data['player'];
      final player = Player(name: playerData['name'], role: playerData['role']);
      Provider.of<LobbyState>(context, listen: false).removePlayer(player);
    });

    dispatcher.register('player_already_in_lobby', (data) {
      _showError("Player is already in another lobby.");
    });
  }

  static void _showError(String message) {
    final context = rootNavigatorKey.currentContext;

    if (context == null) {
      print("Context not available for dialog.");
      return;
    }

    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text("Error"),
        content: Text(message),
        actions: [
          TextButton(
            onPressed: () {
              // Pop twice to remove the loading screen as well
              Navigator.pop(ctx);
              Navigator.pop(ctx);
              },
            child: const Text("OK"),
          ),
        ],
      ),
    );
  }
}
