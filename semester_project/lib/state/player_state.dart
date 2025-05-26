import 'package:flutter/material.dart';
import 'package:semester_project/models/player.dart';

class PlayerState extends ChangeNotifier {
  String? username;
  String? nickname;
  Player? player;

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

}
