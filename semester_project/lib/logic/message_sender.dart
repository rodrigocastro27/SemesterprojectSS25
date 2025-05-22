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

  static void joinLobby(String lobbyId, String username, String nickname) {
    webSocketService.send("join_lobby", {
      "lobbyId": lobbyId,
      "username": username,
      "nickname": nickname,
    });
  }

  static void leaveLobby(String lobbyId, String username) {
    webSocketService.send("exit_lobby", {
      "lobbyId": lobbyId,
      "username": username,
    });
  }

  static void deleteLobby(String lobbyId) {
    webSocketService.send("delete_lobby", {
      "lobbyId": lobbyId,
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
