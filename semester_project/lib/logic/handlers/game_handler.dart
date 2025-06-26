import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:latlong2/latlong.dart';
import 'package:semester_project/logic/action_dispatcher.dart';
import 'package:semester_project/logic/message_sender.dart';
import 'package:semester_project/models/ability_type.dart';
import 'package:semester_project/models/player.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/state/game_state.dart';
import 'package:semester_project/state/lobby_state.dart';
import '../action_dispatcher.dart';
import 'package:semester_project/state/player_state.dart';
import 'package:semester_project/services/navigation_service.dart';
import 'package:semester_project/widgets/mvp_overlay.dart';


class GameActions {
  static void register(
    ServerActionDispatcher dispatcher,
    BuildContext context,
  ) {

    dispatcher.register('game_started', (data) {
      Provider.of<LobbyState>(context, listen: false).startGame(context);
    });

    dispatcher.register("location_request", (data) {
      final gameState = Provider.of<GameState>(context, listen: false);
      gameState.updatePosition(context);

      final location = gameState.getCurrentPosition();
      final name =
          Provider.of<PlayerState>(context, listen: false).getUsername();
      final lobbyId =
          Provider.of<LobbyState>(context, listen: false).getLobbyId();

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
            return Player(name: playerData['name'], role: "", nickname: '')
              ..position = LatLng(playerData['lat'], playerData['lon']);
          }).toList();

      gameState.updateHidersLocation(updatedPlayers);
    });

    dispatcher.register("time_update", (data) {
      final gameState = Provider.of<GameState>(context, listen: false);

      final timeString = data["time"] as String;
      final offsetString = data["time_offset"] as String;

      print("📩 Received time_update:");
      print("  -> Raw time: $timeString");
      print("  -> Raw time_offset: $offsetString");

      try {
        final durationParts = timeString.split(":").map(int.parse).toList();
        if (durationParts.length != 3) {
          print("❌ Invalid time format received: $timeString");
          return;
        }

        final duration = Duration(
          hours: durationParts[0],
          minutes: durationParts[1],
          seconds: durationParts[2],
        );
        print("✅ Parsed duration: $duration");

        final serverOffset = DateTime.parse(offsetString);
        print("✅ Parsed time_offset as DateTime: $serverOffset");

        gameState.updateGameTimer(duration, serverOffset);
      } catch (e, stack) {
        print("❌ Error parsing time_update message: $e");
        print(stack);
      }
    });

    dispatcher.register("task_started", (data) {

      final gameState = Provider.of<GameState>(context, listen: false);

      final taskName = data['name'];

      gameState.startTask(taskName);
    });

    dispatcher.register("game_ended", (data) {
      final gameState = Provider.of<GameState>(context, listen: false);
       final winner = data['winner'];
       gameState.setWinners(winner);
       gameState.stopGame();
    });


    //TASKS ---------------------------------------
    dispatcher.register("task_update", (data) {

      final gameState = Provider.of<GameState>(context, listen: false);

      final taskName = data['taskName'];
      final update = data['update'];
      final updateType = update['type'];

      switch (taskName) {
        case 'ClickingRace': {
          if (updateType == 'time_out') {
            gameState.finishTask(true);
          }
        }
        default: print("NO task specified in update");
      }

      Provider.of<GameState>(context, listen: false).updatePayload(update);
    });

    dispatcher.register("task_result", (data) {
      final gameState = Provider.of<GameState>(context, listen: false);
      final winners = data['winners'];
      gameState.setTaskResult(winners);
    });
    //ABILITIES -----------------------------------
    dispatcher.register("gained_ability", (data) 
    {
      final playerState = Provider.of<PlayerState>(context, listen: false);
      final role = playerState.getPlayer()!.role;
      final abilityName = data['ability'] as String;
      
      rootNavigatorKey.currentState?.push(
        MaterialPageRoute(
          builder: (_) => MVPOverlay(abilityMessage: "You won a new ability! -> $abilityName"),
        ),
      );

      print("ROLE: $role");
      if (role == "hider")
      {
        playerState.addHiderAbilityByName(abilityName);
      } else {
        playerState.addSeekerAbilityByName(abilityName);
      }
    });

    dispatcher.register("used_ability", (data) 
    {
      // This is not really used because from Flutter you already know you used it
      // In case some of the tasks need some server validation before being used
        final abilityName = data['ability'] as String;

    });


    dispatcher.register("qr_switch", (data){  //when user performs a qr code switch action

      final newName = data['newCode'];
      Provider.of<PlayerState>(context, listen: false).setQrCode(newName); 

    });

    dispatcher.register("qr_scanned", (data){  //when user's qr code (after being switched) gets scanned. possibly should centralize all clients to recieve this message

      String? name = Provider.of<PlayerState>(context, listen: false).getUsername();
      Provider.of<PlayerState>(context, listen: false).setQrCode(name!); 
    });




    //ELIMINATION ---------------------------------
    dispatcher.register("eliminated_player", (data){
      // To then handle notifying everyone that a player has been eliminated (not implemented)
        final gameState = Provider.of<GameState>(context, listen: false);
    });

    dispatcher.register("current_player_eliminated", (data){
        final gameState = Provider.of<GameState>(context, listen: false);
        gameState.playerCatpured();
        print("you are eliminated!!!");
    });
    
  }
}
