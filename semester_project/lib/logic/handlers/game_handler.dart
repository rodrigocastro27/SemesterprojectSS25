import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'package:semester_project/state/lobby_state.dart';
import '../action_dispatcher.dart';

class GameActions {
  static void register(ServerActionDispatcher dispatcher, BuildContext context) {
    dispatcher.register('game_started', (data) {
      Provider.of<LobbyState>(context, listen: false).startGame();
    });
  }
}