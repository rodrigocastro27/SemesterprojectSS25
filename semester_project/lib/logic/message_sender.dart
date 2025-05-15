import 'dart:math';

import 'package:semester_project/main.dart';

class MessageSender {
  static void createLobby(String lobbyId, String name, int id) {
    webSocketService.send("create_lobby", {
      "lobbyId": lobbyId,
      "name": name,
      "id": id,
    });
  }

  static void joinLobby(String lobbyId, String name, int id) {
    webSocketService.send("join_lobby", {
      "lobbyId": lobbyId,
      "name": name,
      "id": id,
    });
  }

  static void leaveLobby(String lobbyId, String name, int id) {
    webSocketService.send("leave_lobby", {
      "lobbyId": lobbyId,
      "name": name,
      "id": id,
    });
  }

  static void updatePosition(String name, String lobbyId, double lat, double lon) {
    webSocketService.send("update_position", {
      "name": name,
      "lobbyId": lobbyId,
      "lat": lat,
      "lon": lon,
    });
  } 

  static void startGame(String lobbyId)
  {
    webSocketService.send("start_game", {
      "lobbyId": lobbyId,
    });
  }
}
