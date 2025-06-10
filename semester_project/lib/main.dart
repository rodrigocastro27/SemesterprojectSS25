import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/logic/action_dispatcher.dart';
import 'package:semester_project/logic/handlers/game_handler.dart';
import 'package:semester_project/logic/handlers/lobby_handler.dart';
import 'package:semester_project/logic/handlers/player_handler.dart';
import 'package:semester_project/router/router.dart';
import 'package:semester_project/services/websocket_service.dart';
import 'package:semester_project/state/lobby_state.dart';
import 'package:semester_project/state/player_state.dart';
import 'package:semester_project/state/game_state.dart';

final dispatcher = ServerActionDispatcher();
late WebSocketService webSocketService;

void main() {
  runApp(
    MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => PlayerState()),
        ChangeNotifierProvider(create: (_) => LobbyState()),
        ChangeNotifierProvider(create: (_) => GameState()),
      ],
      child: const MyApp(),
    ),
  );
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    setupActionHandlers(context);

    final playerState = Provider.of<PlayerState>(context, listen: false);
    final lobbyState = Provider.of<LobbyState>(context, listen: false);
    final gameState = Provider.of<GameState>(context, listen: false);
    final router = createRouter(playerState, lobbyState, gameState);

    webSocketService = WebSocketService(dispatcher);

    webSocketService.setOnConnect(() {
      playerState.setConnectionState(true);
      print("✅ WebSocket connected - player marked as connected.");
    });
    // Set disconnect callback here
    webSocketService.setOnDisconnect(() {
      playerState.setConnectionState(false); // You need to implement this method in PlayerState
      // Optionally: show a toast/snackbar or navigate away
      print("⚠️ WebSocket disconnected - player marked as disconnected.");
    });

    webSocketService.connect('wss://c06a-193-170-132-8.ngrok-free.app/ws');

    return MaterialApp.router(
      debugShowCheckedModeBanner: false,
      routerConfig: router,
    );
  }
}

void setupActionHandlers(BuildContext context) {
  LobbyActions.register(dispatcher, context);
  PlayerActions.register(dispatcher, context);
  GameActions.register(dispatcher, context);
}
