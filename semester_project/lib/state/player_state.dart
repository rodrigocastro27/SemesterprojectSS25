import 'package:flutter/material.dart';

class PlayerState extends ChangeNotifier {
  String? username;
  String? nickname;

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
