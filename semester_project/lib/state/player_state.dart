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

void addHiderAbilityByName(String name) {
  print("Adding hider ability");
   final ability = HiderAbility.values.firstWhere(
    (e) => e.name == name);
  
  hiderAbilities.add(ability);
  notifyListeners();
}

void addSeekerAbilityByName(String name) {
    print("Adding seeker ability");
    final ability = SeekerAbility.values.firstWhere(
    (e) => e.name == name);

  seekerAbilities.add(ability);
  notifyListeners();
}


 
  void removeHiderAbility(HiderAbility ability) {
    hiderAbilities.remove(ability);
    notifyListeners();
  }
  
  void removeSeekerAbility(SeekerAbility ability) {
    seekerAbilities.remove(ability);
    notifyListeners();
  }

  void useAbility(Enum ability, BuildContext context) {
    final lobbyState = Provider.of<LobbyState>(context, listen: false);
    var lobbyId = lobbyState.lobbyId;   
     
    MessageSender.sendAbilityUsed(lobbyId!, username!, ability.name);

    //possibly improve with dictionary 
    
    switch(ability) {
      case HiderAbility.HidePing:
        // hidePlayer();  // NOT REALLY USED, maybe for future UI
        removeHiderAbility(HiderAbility.HidePing);
      case HiderAbility.SwapQr:
        MessageSender.sendAbilityUsed(lobbyId, username!, ability.name);
        removeHiderAbility(HiderAbility.SwapQr);
      // Add more hider abilities here
      // ...

      case SeekerAbility.GainPing:
        addPing();
        removeSeekerAbility(SeekerAbility.GainPing);
      case SeekerAbility.HiderSound:
        makeHidersPhonesSound(context);
        removeSeekerAbility(SeekerAbility.HiderSound);
      // Add more seeker abilities here
      // ...
    }


  }

  void hidePlayer() { //currently not used, controlled in server
    hiderIsHidden = true;
    notifyListeners();
  }

  void swapQRcode(BuildContext context) //not used anymore -> refactored to be controlled on server side.
  {
    final gameState = Provider.of<GameState>(context, listen: false);
    final random = Random();
    final chosenPlayer =  gameState.hiders[random.nextInt(gameState.hiders.length)];
   // fakeName = chosenPlayer.name;
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


  void setQrCode(String qr){
    _player!.qrCode = qr;
  }

}
