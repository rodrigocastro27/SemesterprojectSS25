import 'package:flutter/material.dart';
import 'package:semester_project/logic/action_dispatcher.dart';
import 'package:semester_project/logic/handlers/lobby_handler.dart';
import 'package:semester_project/pages/home_page.dart';
import 'package:semester_project/services/websocket_service.dart';


final dispatcher = ServerActionDispatcher();
late WebSocketService webSocketService;

final GlobalKey<NavigatorState> navigatorKey = GlobalKey<NavigatorState>();   //basic global navigation... look into it!



void main() {

  setupActionHandlers();

  webSocketService = WebSocketService(dispatcher);
  webSocketService.connect('wss://91a4-193-170-134-240.ngrok-free.app/ws');  //ngrok link

  runApp(const MyApp());
}


void setupActionHandlers() {

    LobbyActions.register(dispatcher);
  // Add more handlers here as needed
}


class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
   return MaterialApp(
      navigatorKey: navigatorKey,
      debugShowCheckedModeBanner: false,
      home: HomePage(),
    );
  }
}


