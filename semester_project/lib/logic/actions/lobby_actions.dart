import 'package:flutter/material.dart';
import 'package:semester_project/main.dart';
import 'package:semester_project/pages/lobby%20pages/lobby_page.dart';

import '../action_dispatcher.dart';

class LobbyActions {
  static void register(ServerActionDispatcher dispatcher) {
    dispatcher.register('lobby_joined', _handleLobbyJoined);
    dispatcher.register('lobby_created', _handleLobbyCreated);
  }

  static void _handleLobbyJoined(Map<String, dynamic> data) {
    final lobbyId = data['lobbyId'];
    final players = data['players'] as List<dynamic>;

    print("✅ Successfully joined lobby: $lobbyId");
    for (final player in players) {
      print(" - ${player['name']} (ID: ${player['id']})");
    }

    navigatorKey.currentState?.pop();
    navigatorKey.currentState?.push(
      MaterialPageRoute(builder: (context) => LobbyPage(lobbyId: lobbyId)),
    );

    // TODO: update state, UI, etc.
  }

  static void _handleLobbyCreated(Map<String, dynamic> data) {
    final lobbyId = data['lobbyId'];
    final isHost = data['host'] == true;

    print("🎉 Lobby created successfully: $lobbyId");
    print("You are the host: $isHost");
    // Close the loading dialog
    navigatorKey.currentState?.pop(); // Close loading dialog

    // Navigate to the LobbyPage
    navigatorKey.currentState?.push(
      MaterialPageRoute(builder: (context) => LobbyPage(lobbyId: lobbyId)),
    );
  }
}
