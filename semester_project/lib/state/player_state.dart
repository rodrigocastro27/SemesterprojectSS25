import 'dart:math';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/logic/message_sender.dart';
import 'package:semester_project/models/player.dart';
import 'package:semester_project/models/ability_type.dart';
import 'package:semester_project/state/game_state.dart';
import 'package:semester_project/state/lobby_state.dart';
import 'package:audioplayers/audioplayers.dart' as audioplayers;

class PlayerState extends ChangeNotifier {
  String? username;
  String? nickname;
  bool isConnected = false;
  bool isOnline = false;
  Player? _player;
  List<HiderAbility> hiderAbilities = [];
  List<SeekerAbility> seekerAbilities = [];
  bool hiderIsHidden = false;
  String? fakeName;
  int pings = 1;

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

  void removeHiderAbility(HiderAbility ability) {
    hiderAbilities.remove(ability);
    notifyListeners();
  }

  void addSeekerAbility() {
    final values = SeekerAbility.values;
    final randomIndex = Random().nextInt(values.length);
    seekerAbilities.add(values[randomIndex]);
    print("Player ${_player!.name} gained ability: ${values[randomIndex]}");
  }

  void removeSeekerAbility(SeekerAbility ability) {
    seekerAbilities.remove(ability);
    notifyListeners();
  }

  void useAbility(Enum ability, BuildContext context) {
    switch(ability) {
      case HiderAbility.hidePing:
        hidePlayer();
        removeHiderAbility(HiderAbility.hidePing);
      case HiderAbility.swapQR:
        swapQRcode(context);
        removeHiderAbility(HiderAbility.swapQR);
      // Add more hider abilities here
      // ...

      case SeekerAbility.gainPing:
        addPing();
        removeSeekerAbility(SeekerAbility.gainPing);
      case SeekerAbility.hiderSound:
        makeHidersPhonesSound(context);
        removeSeekerAbility(SeekerAbility.hiderSound);
      // Add more seeker abilities here
      // ...
    }


  }

  void hidePlayer() {
    hiderIsHidden = true;
    notifyListeners();
  }

  void swapQRcode(BuildContext context) {
    final gameState = Provider.of<GameState>(context, listen: false);
    final random = Random();
    final chosenPlayer =  gameState.hiders[random.nextInt(gameState.hiders.length)];
    fakeName = chosenPlayer.name;
    notifyListeners();
  }

  void addPing() {
    pings += 1;
    notifyListeners();
  }

  void removePing() {
    pings -= 1;
    notifyListeners();
  }

  void makeHidersPhonesSound(BuildContext context) {
    final lobbyState = Provider.of<LobbyState>(context, listen: false);
    var lobbyId = lobbyState.lobbyId;
    MessageSender.makeHidersPhonesSound(lobbyId!);
  }
}
