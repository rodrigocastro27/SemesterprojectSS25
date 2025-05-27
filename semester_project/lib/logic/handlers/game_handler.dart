import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:semester_project/logic/action_dispatcher.dart';
import 'package:semester_project/logic/message_sender.dart';
import 'package:semester_project/models/player.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/state/game_state.dart';
import 'package:semester_project/state/lobby_state.dart';
import '../action_dispatcher.dart';
import 'package:semester_project/state/player_state.dart';

class GameActions {
  static void register(
    ServerActionDispatcher dispatcher,
    BuildContext context,
  ) {
    dispatcher.register('game_started', (data) 
    {
      Provider.of<LobbyState>(context, listen: false).startGame(context);
    });
    dispatcher.register("location_request", (data) {
      final gameState = Provider.of<GameState>(context, listen: false);
      gameState.updatePosition(context);

      final location = gameState.getCurrentPosition();
      final name = Provider.of<PlayerState>(context,listen: false).getUsername();
      final lobbyId = Provider.of<LobbyState>(context, listen: false).getLobbyId();

      if (name != null && lobbyId != null && location != null) {
        MessageSender.updatePosition(
          name,
          lobbyId,
          location.latitude,
          location.longitude,
        );
      }
    });

    dispatcher.register("location_update_list", (data) {
      
      var gameState = Provider.of<GameState>(context, listen: false);
      
      //handle ping button change of state logic for other seekers
      gameState.handlePingStartedFromServer();

      final playersData = data['players'] as List<dynamic>;


      final updatedPlayers =
              playersData.map((playerData) {
            return Player(
              name: playerData['name'],
              role: playerData['role'],
              nickname: '',
            );
          }).toList();

       gameState.updateHidersLocation(updatedPlayers);
    });
  }
}
