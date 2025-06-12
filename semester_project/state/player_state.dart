import 'package:flutter/material.dart';
import 'package:semester_project/models/player.dart';

class PlayerState extends ChangeNotifier {
  String? username;
  String? nickname;
  bool isConnected = false;
  bool isOnline = false;
  Player? _player;

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

  String? getNickname(){
    return nickname;
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

  void setPlayer(Player player) {
    _player = player;
  }

  Player? getPlayer(){
    return _player;
  }
}