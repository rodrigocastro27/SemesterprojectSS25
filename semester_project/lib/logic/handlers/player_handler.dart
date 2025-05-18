import 'package:flutter/material.dart';
import 'package:semester_project/main.dart';
import 'package:semester_project/pages/home_page.dart';

import '../action_dispatcher.dart';

class PlayerActions {

  // Messages recieved from the server and how to handle them
  static void register(ServerActionDispatcher dispatcher) {
    dispatcher.register('player_registered', _handlePlayerRegistered);
  }

  static void _handlePlayerRegistered(Map<String, dynamic> data) {
    final id = data['id'];
    final username = data['user'];

    navigatorKey.currentState?.pop();
    navigatorKey.currentState?.push(
      MaterialPageRoute(builder: (_) => HomePage(username: username)),
    );
  }
}
