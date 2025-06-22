import 'package:flutter/material.dart';
import 'package:semester_project/models/player.dart';
import 'package:semester_project/models/user.dart';

class PlayerState extends ChangeNotifier {
  String? username;
  String? nickname;
  bool isConnected = false;
  bool isOnline = false;
  Player? player;

  User? _authenticatedUser;
  User? get authenticatedUser => _authenticatedUser;
  void register(String name) {
    username = name;
    notifyListeners();
  }

  void setNickname(String nickname) {
    nickname = nickname;
    notifyListeners();
  }

  String? getUsername() {
    return username;
  }

  void setConnectionState(bool isConnected) {
    this.isConnected = isConnected;
    if (!isConnected) {
      setOnline(false);
    }
    notifyListeners();
  }

  void setOnline(bool isOnline) {
    this.isOnline = isOnline;
  }

   void setAuthenticatedUser(User user) {
    _authenticatedUser = user;
    
    username = user.email;
    notifyListeners();
  }

  
  void clearAuthenticatedUser() {
    _authenticatedUser = null;
    username = null; 
    notifyListeners();
  }

  
  void setPlayer(Player newPlayer) { 
    player = newPlayer;
    username = newPlayer.name;
    nickname = newPlayer.nickname; 
    notifyListeners();
  }

}
