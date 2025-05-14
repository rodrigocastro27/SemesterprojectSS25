import 'package:semester_project/main.dart';

class MessageSender {
  static void createLobby(String lobbyId, String name) {
    webSocketService.send("create_lobby", {
      "lobbyId": lobbyId,
      "name": name,
      "id": 10,
    });
  }

  static void joinLobby(String lobbyId, String name) {
    webSocketService.send("join_lobby", {
      "lobbyId": lobbyId,
      "name": name,
      "id": 10,
    });
  }

  static void leaveLobby(String lobbyId, String name) {
    webSocketService.send("leave_lobby", {
      "lobbyId": lobbyId,
      "name": name,
      "id": 10,
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

  static void startGame()
  {
    //start the game
  }
}
