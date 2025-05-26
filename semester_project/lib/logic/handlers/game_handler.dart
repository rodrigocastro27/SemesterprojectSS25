import 'package:flutter/material.dart';
import 'package:semester_project/logic/action_dispatcher.dart';
import 'package:semester_project/logic/message_sender.dart';
import 'package:semester_project/models/player.dart';
import 'package:semester_project/state/lobby_state.dart';
import 'package:semester_project/state/player_state.dart';

class GameActions {
  static void register(
    ServerActionDispatcher dispatcher,
    BuildContext context,
    PlayerState playerState,
    LobbyState lobbyState,
  ) {
    dispatcher.register("location_request", (data) {
      final username = playerState.getUsername();
      final lobbyId = lobbyState.lobbyId;

  
      /*
      final location = playerState.getLocation();


      //send location
      if (username != null && lobbyId != null && location != null) {
        MessageSender.sendLocation(
          username,
          lobbyId,
          location.longitude,
          location.latitude,
        );
      }*/
    });
    
    dispatcher.register("location_update_list", (data){

    /*
        final playersData = data['players'] as List<dynamic>;
        final updatedPlayers = playersData.map((playerData) {
    return Player(
      name: playerData['name'],
      role: playerData['role'],
      latitude: playerData['latitude'],
      longitude: playerData['longitude'],
    );
  }).toList();

       lobbyState.updatePlayerLocations(updatedPlayers);
    */
    });
  }
}
