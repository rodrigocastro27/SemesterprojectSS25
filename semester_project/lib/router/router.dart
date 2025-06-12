import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:semester_project/pages/authentication_page.dart';
import 'package:semester_project/pages/end_page.dart';
import 'package:semester_project/pages/home_page.dart';
import 'package:semester_project/pages/lobby%20pages/lobby_page.dart';
import 'package:semester_project/pages/game%20pages/map_page.dart';
import 'package:semester_project/services/navigation_service.dart';
import 'package:semester_project/state/game_state.dart';
import 'package:semester_project/state/player_state.dart';
import 'package:semester_project/state/lobby_state.dart';

GoRouter createRouter(
  PlayerState playerState,
  LobbyState lobbyState,
  GameState gameState,
) {
  return GoRouter(
    navigatorKey: rootNavigatorKey,
    initialLocation: '/auth',
    refreshListenable: Listenable.merge([playerState, lobbyState, gameState]),

    redirect: (context, state) {
      
      
      final currentLocation = state.fullPath?.split('?').first;

      final hasUsername = playerState.username != null;
      final inLobby = lobbyState.lobbyId != null;
      final playing = lobbyState.playing;
      final gameEnded = gameState.gameEnded;

      // 1. If no username => go to /auth
      if (!hasUsername) {
        playerState.setOnline(false);
        if (currentLocation != '/auth') return '/auth';
        return null;
      }

      // 2. If game ended => go to /end
      if (gameEnded&&playing) {
        if (currentLocation != '/end') return '/end';
        return null;
      }

      // 3. If user is in a lobby and playing => /game
      if (hasUsername && inLobby && playing && !gameEnded) {
        if (currentLocation != '/game') return '/game';
        return null;
      }

      // 4. If user is in lobby but not playing => /lobby
      if (hasUsername && inLobby && !playing && !gameEnded) {
        if (currentLocation != '/lobby') return '/lobby';
        return null;
      }

      // 5. If user has username, not in lobby and not playing => /home
      if (hasUsername && !inLobby && !playing) {
        if (currentLocation != '/home') {
          playerState.setOnline(true);
          return '/home';
        }
        return null;
      }

      // 6. Default: no redirect
      return null;
    },

    routes: [
      GoRoute(path: '/auth', builder: (context, state) => const AuthPage()),
      GoRoute(
        path: '/home',
        builder: (context, state) => HomePage(username: playerState.username!),
      ),
      GoRoute(path: '/lobby', builder: (context, state) => const LobbyPage()),

      GoRoute(path: '/game', builder: (context, state) => const MapPage()),


      GoRoute(
        path: '/end',
        builder:
            (context, state) => const EndGamePage(
              resultMessage: "Game Over",
              additionalInfo: "You were removed from the game.",
            ),
      ),
    ],
  );
}
