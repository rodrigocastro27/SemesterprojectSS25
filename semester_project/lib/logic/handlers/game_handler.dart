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
import 'package:latlong2/latlong.dart';

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
      final name = Provider.of<PlayerState>(context, listen: false).getUsername();
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

    // Handler for ping_activated message
    dispatcher.register("ping_activated", (data) {
      var gameState = Provider.of<GameState>(context, listen: false);
      
      // Extract the requesting player's ID
      final requestingPlayerId = data['requestingPlayerId'];
      
      // Process the hiders' locations
      final playersData = data['players'] as List<dynamic>;
      final updatedPlayers = playersData.map((playerData) {
        return Player(
          name: playerData['name'],
          role: "hider",
          nickname: '',
        )..position = LatLng(
            playerData['latitude'],
            playerData['longitude'],
          );
      }).toList();

      // Update game state with ping information
      gameState.handleUniversalPingActivated(requestingPlayerId, updatedPlayers);
    });
    
    // Handler for ping_rejected message
    dispatcher.register("ping_rejected", (data) {
      var gameState = Provider.of<GameState>(context, listen: false);
      final reason = data['reason'];
      
      // Update game state to show ping was rejected
      gameState.handlePingRejected(reason);
    });
    
    // Handler for ping_ended message
    dispatcher.register("ping_ended", (data) {
      var gameState = Provider.of<GameState>(context, listen: false);
      final requestingPlayerId = data['requestingPlayerId'];
      
      // Update game state to end ping
      gameState.handlePingEnded(requestingPlayerId);
    });
    
    // Handler for ping_cooldown_ended message
    dispatcher.register("ping_cooldown_ended", (data) {
      var gameState = Provider.of<GameState>(context, listen: false);
      
      // Update game state to end cooldown
      gameState.handlePingCooldownEnded();
    });
    
    // Keep the old handler for backward compatibility during transition
    dispatcher.register("location_update_list", (data) {
      var gameState = Provider.of<GameState>(context);
      
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

