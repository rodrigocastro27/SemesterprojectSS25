import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/models/player.dart';
import 'package:semester_project/state/lobby_state.dart';
import '../action_dispatcher.dart';
import 'package:semester_project/services/navigation_service.dart';
import 'package:semester_project/state/player_state.dart';

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

    dispatcher.register('start_game', (data) {
      Provider.of<LobbyState>(context, listen: false).startGame();
    });

    dispatcher.register('failed_lobby', (data) {
      _showError("Could not join lobby because it doesn't exist.");
    });

    dispatcher.register('leave_lobby', (data) {
      final playerData = data['player'];
      final player = Player(name: playerData['name'], role: playerData['role']);
      Provider.of<LobbyState>(context, listen: false).leaveLobby(player);
    });

    dispatcher.register('player_already_in_lobby', (data) {
      _showError("Player is already in another lobby.");
    });

    dispatcher.register('new_host', (data) {
      // final username = data['player'];
      Provider.of<LobbyState>(context, listen: false).setNewHost();
    });

    dispatcher.register('lobby_deleted', (data) {
      _showError("The lobby you were in was deleted!");
      Provider.of<LobbyState>(context, listen: false).clearLobby();
    });

    dispatcher.register("player_list", (data){
        final playersData = data['players'] as List<dynamic>;
        final newList = playersData.map((playerData) {
                return Player(
            name: playerData['name'],
            role: playerData['role'],
          );
        }).toList();


        Provider.of<LobbyState>(context, listen: false).updatePlayerList(newList);
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
