import 'package:flutter/material.dart';
import 'package:semester_project/models/player.dart';

class LobbyState extends ChangeNotifier {
  String? lobbyId;
  bool isHost = false;
  List<Player> players = [];
  bool playing = false;

  void leaveLobby(player) {
    removePlayer(player);
    lobbyId = null;
    notifyListeners();
  }

  void setLobby(String id, List<Player> newPlayers, {bool isHost = false}) {
    lobbyId = id;
    players = newPlayers;
    this.isHost = isHost;
    notifyListeners();
  }

  void addPlayer(Player player) {
    if (!players.any((p) => p.name == player.name)) {
      players.add(player);
      notifyListeners();
    }
  }

  void removePlayer(Player player) {
    if (players.any((p) => p.name == player.name)) {
      players.remove(player);
      notifyListeners();
    }
  }

  void clearLobby() {
    lobbyId = null;
    players.clear();
    isHost = false;
    notifyListeners();
  }

  void setNewHost() {
    isHost = true;
    notifyListeners();
  }

  void startGame() {
    playing = true;
    notifyListeners();
  }

  void updatePlayerList(List<Player> newList){
      players= newList;
      notifyListeners();
  }
}
