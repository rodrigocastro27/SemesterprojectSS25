// router.dart

import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:semester_project/pages/authentication_page.dart';
import 'package:semester_project/pages/home_page.dart';
import 'package:semester_project/pages/lobby%20pages/lobby_page.dart';
import 'package:semester_project/pages/map_page.dart';
import 'package:semester_project/services/navigation_service.dart';
import 'package:semester_project/state/player_state.dart';
import 'package:semester_project/state/lobby_state.dart';

GoRouter createRouter(PlayerState playerState, LobbyState lobbyState) {
  return GoRouter(
    navigatorKey: rootNavigatorKey,
    initialLocation: '/auth',
    refreshListenable: Listenable.merge([playerState, lobbyState]),
    redirect: (context, state) {
      final hasUsername = playerState.username != null;
      final inLobby = lobbyState.lobbyId != null;
      final playing = lobbyState.playing;

      // final isAuth = state.fullPath == '/auth';
      final isHome = state.fullPath == '/home';
      // final isLobby = state.fullPath == '/lobby';

      if (!hasUsername) return '/auth';
      if (hasUsername && !inLobby && !isHome) return '/home';
      if (hasUsername && inLobby) return '/lobby';
      if (hasUsername && inLobby && playing) return '/game';

      return null;
    },
    routes: [
      GoRoute(
        path: '/auth',
        builder: (context, state) => const AuthPage(),
      ),
      GoRoute(
        path: '/home',
        builder: (context, state) => HomePage(username: playerState.username!),
      ),
      GoRoute(
        path: '/lobby',
        builder: (context, state) => const LobbyPage(),
      ),
      GoRoute(
        path: '/game',
        builder: (context, state) => const MapPage(),
      ),
    ],
  );
}
