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
  static void createLobby(String lobbyId, String username, String role) {
    webSocketService.send("create_lobby", {
      "lobbyId": lobbyId,
      "username": username,
      "role": role,

    });
  }

  static void joinLobby(String lobbyId, String username, String nickname, String role) {
    webSocketService.send("join_lobby", {
      "lobbyId": lobbyId,
      "username": username,
      "nickname": nickname,
      "role": role,
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
      "username": name,
      "lobbyId": lobbyId,
      "lat": lat,
      "lon": lon,
    });
  } 

  static void setMapCenter(String lobbyId, double lat, double lon) {
    webSocketService.send("set_map_center", {
      "lobbyId": lobbyId,
      "latitude": lat,
      "longitude": lon,
    });
  }

  static void startGame(String lobbyId)
  {
    webSocketService.send("start_game", {
      "lobbyId": lobbyId,
    });
  }

  static void pingRequest(String name, String lobbyId) {
    webSocketService.send("ping_request", {
      "username": name,
      "lobbyId": lobbyId,
    });
  } 

  static void eliminatePlayer(String name, String lobbyId) {
    webSocketService.send("player_eliminated", {
      "username": name,
      "lobbyId": lobbyId,
    });
  } 
  
  // TASKS ---------------------------------------------------------------
  static void startTask(String name, String lobbyId) {
    webSocketService.send("start_task", {
      "username": name,
      "lobbyId": lobbyId,
    });
  }

  static void sendTaskUpdate(String lobbyId, String username, Map<String,dynamic> payload) {
    webSocketService.send("update_task", {
      "username" : username,
      "lobbyId" : lobbyId,
      "payload" : payload,
    });
  }

  static void makeHidersPhonesSound(String lobbyId) {
    webSocketService.send("make_hiders_phone_sound", {
      "lobbyId": lobbyId,
    });
  }
  
}
