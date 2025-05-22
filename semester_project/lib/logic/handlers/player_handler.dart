import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/state/lobby_state.dart';
import 'package:semester_project/state/player_state.dart';
import '../action_dispatcher.dart';

class PlayerActions {
  static void register(ServerActionDispatcher dispatcher, BuildContext context) {
    dispatcher.register('player_registered', (data) {
      final username = data['user'];
      Provider.of<PlayerState>(context, listen: false).register(username);
    });
  }
}
