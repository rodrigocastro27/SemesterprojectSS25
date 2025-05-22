import 'package:flutter/material.dart';

class PlayerState extends ChangeNotifier {
  String? username;

  void register(String name) {
    username = name;
    notifyListeners();
  }

  String? getUsername() {
    return username;
  }
}
