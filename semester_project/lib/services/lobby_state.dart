import 'package:semester_project/models/player.dart';

class LobbyState {
  static final LobbyState instance = LobbyState._();
  LobbyState._();

  String? lobbyId;
  bool isHost = false;
  List<Player> players = [];

  void setLobby(String id, List<Player> newPlayers, {bool isHost = false}) {
    lobbyId = id;
    players = newPlayers;
    this.isHost = isHost;
  }

  void updatePlayers(List<Player> updated) {
    players = updated;
  }
}
