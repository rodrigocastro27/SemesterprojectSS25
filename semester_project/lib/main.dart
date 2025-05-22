import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/logic/action_dispatcher.dart';
import 'package:semester_project/logic/handlers/lobby_handler.dart';
import 'package:semester_project/logic/handlers/player_handler.dart';
import 'package:semester_project/router/router.dart';
import 'package:semester_project/services/websocket_service.dart';
import 'package:semester_project/state/lobby_state.dart';
import 'package:semester_project/state/player_state.dart';

final dispatcher = ServerActionDispatcher();
late WebSocketService webSocketService;

void main() {
  runApp(
    MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => PlayerState()),
        ChangeNotifierProvider(create: (_) => LobbyState()),
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
    final router = createRouter(playerState, lobbyState);

    webSocketService = WebSocketService(dispatcher);
    webSocketService.connect('wss://c202-193-170-132-8.ngrok-free.app/ws');

    return MaterialApp.router(
      debugShowCheckedModeBanner: false,
      routerConfig: router,
    );
  }
}

void setupActionHandlers(BuildContext context) {
  LobbyActions.register(dispatcher, context);
  PlayerActions.register(dispatcher, context);
}
