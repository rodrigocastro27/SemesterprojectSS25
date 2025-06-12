import 'dart:math';
import 'package:flutter/material.dart';
import 'package:semester_project/models/player.dart';
import 'package:semester_project/models/ability_type.dart';

class PlayerState extends ChangeNotifier {
  String? username;
  String? nickname;
  bool isConnected = false;
  bool isOnline = false;
  Player? _player;
  List<HiderAbility> hiderAbilities = [];
  List<SeekerAbility> seekerAbilities = [];

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

  void addHiderAbility() {
    final values = HiderAbility.values;
    final randomIndex = Random().nextInt(values.length);
    hiderAbilities.add(values[randomIndex]);
    print("Player ${_player!.name} gained ability: ${values[randomIndex]}");
  }

  void addSeekerAbility() {
    final values = SeekerAbility.values;
    final randomIndex = Random().nextInt(values.length);
    seekerAbilities.add(values[randomIndex]);
    print("Player ${_player!.name} gained ability: ${values[randomIndex]}");
  }
}
