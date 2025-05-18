import 'package:semester_project/main.dart';

class MessageSender {

  // PLAYER ---------------------------------------------------------------
  static void loginPlayer(String deviceId, String username) {
    webSocketService.send("login_player", {
      "deviceId": deviceId,
      "username": username,
      });
  }

  // LOBBIES --------------------------------------------------------------
  static void createLobby(String lobbyId, String username) {
    webSocketService.send("create_lobby", {
      "lobbyId": lobbyId,
      "username": username,
    });
  }

  static void joinLobby(String lobbyId, String username) {
    webSocketService.send("join_lobby", {
      "lobbyId": lobbyId,
      "username": username,
    });
  }

  static void leaveLobby(String lobbyId, String username) {
    webSocketService.send("leave_lobby", {
      "lobbyId": lobbyId,
      "username": username,
    });
  }

  // GAME -------------------------------------------------------------------------------
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
