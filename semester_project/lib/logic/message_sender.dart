import 'package:flutter/material.dart';
import 'package:semester_project/main.dart'; 


class MessageSender {

  // --- AUTHENTICATION  ------------------------------------------------------------------------
  static void register({
    required String email,
    required String password,
    String? googleId
  }) {

    webSocketService.send("register_request", {
      "email": email,
      "password": password,
      if (googleId != null) "googleId": googleId, 
    });
    print("📤 Sent register_request for $email");
  }

  static void login({
    required String email,
    required String password,
  }) {
    webSocketService.send("login_request", {
      "email": email,
      "password": password,
    });
    print("📤 Sent login_request for $email");
  }

  static void requestPasswordReset({
    required String email,
  }) {
    webSocketService.send("password_reset_request", {
      "email": email,
    });
    print("📤 Sent password_reset_request for $email");
  }

  static void updatePassword({
    required String token,
    required String newPassword,
  }) {
    webSocketService.send("password_update", {
      "token": token,
      "newPassword": newPassword,
    });
    print("📤 Sent password_update request with token $token");
  }

  // --- PLAYER MESSAGES (Existing) ---------------------------------------------------------------
  static void loginPlayer(String deviceId, String username) {
    webSocketService.send("login_player", {
      "deviceId": deviceId,
      "username": username,
    });
  }

  // --- LOBBIES MESSAGES (Existing) --------------------------------------------------------------
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

  // --- GAME MESSAGES (Existing) -------------------------------------------------------------------------------
  static void updatePosition(String name, String lobbyId, double lat, double lon) {
    webSocketService.send("update_position", {
      "username": name,
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

  static void pingRequest(String name, String lobbyId) {
    webSocketService.send("ping_request", {
      "username": name,
      "lobbyId": lobbyId,
    });
  } 
}
