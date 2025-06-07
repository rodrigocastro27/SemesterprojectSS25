import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/logic/message_sender.dart';
import 'package:semester_project/models/ping_state.dart';
import 'package:latlong2/latlong.dart';
import 'package:geolocator/geolocator.dart';
import 'package:semester_project/models/player.dart';
import 'package:semester_project/state/lobby_state.dart';
import 'package:semester_project/state/player_state.dart'; // geolocator package

class GameState extends ChangeNotifier {
  bool isHider = false;
  PingState pingState = PingState.idle;
  int cooldownSeconds = 10;
  LatLng? userLocation;
  
  // Track the current pinging seeker
  String? currentPingingSeekerId;
  bool isPingRejected = false;
  String pingRejectionReason = "";

  //tbd architecture
  List<Player> hiders = [];
  List<Player> seekers = [];

  List<Player> players = [];

  void initGame(BuildContext context) {
    players = Provider.of<LobbyState>(context).getPlayerList();

    //maybe unecessary:

    for (var p in players) {
      if (p.role == "hider") {
        hiders.add(p);
      } else if (p.role == "seeker") {
        seekers.add(p);
      }

      print("Total players: ${players.length}");
      print("Hiders: ${hiders.map((p) => p.name).toList()}");
      print("Seekers: ${seekers.map((p) => p.name).toList()}");

      //other game starting logic, maybe load settings
    }
  }

  void setRole(bool hider) {
    isHider = hider;
    pingState = hider ? PingState.pinging : PingState.idle;
    notifyListeners();
  }

  void setLocation(LatLng location) {
    userLocation = location;
    notifyListeners();
  }

  void startPing(BuildContext context)
   {
    // Only allow starting a ping if we're in idle state
    if (pingState != PingState.idle) {
      return;
    }
    
    // Set local state to pinging (will be confirmed or rejected by server)
    pingState = PingState.pinging;
    isPingRejected = false;
    notifyListeners();

    final username =
        Provider.of<PlayerState>(context, listen: false).getUsername();
    final lobbyId =
        Provider.of<LobbyState>(context, listen: false).getLobbyId();

    if (lobbyId != null && username != null) {
      MessageSender.pingRequest(username, lobbyId);
    }
  }

  void initLocation(BuildContext context) async {
    bool serviceEnabled = await Geolocator.isLocationServiceEnabled();
    if (!serviceEnabled) return;

    LocationPermission permission = await Geolocator.checkPermission();
    if (permission == LocationPermission.denied) {
      permission = await Geolocator.requestPermission();
    }
    if (permission == LocationPermission.deniedForever ||
        permission == LocationPermission.denied)
      return;

    final position = await Geolocator.getCurrentPosition(
      desiredAccuracy: LocationAccuracy.high,
    );
    setLocation(LatLng(position.latitude, position.longitude));
  }

  void updatePosition(BuildContext context) async {
    final position = await Geolocator.getCurrentPosition(
      desiredAccuracy: LocationAccuracy.high,
    );
    setLocation(LatLng(position.latitude, position.longitude));
  }

  LatLng? getCurrentPosition() {
    return userLocation;
  }

  void updateHidersLocation(List<Player> newList) {
    hiders.clear();
    hiders = newList;
    notifyListeners();
  }

  // Handle universal ping activated
  void handleUniversalPingActivated(String requestingPlayerId, List<Player> updatedHiders) {
    // Set the current pinging seeker
    currentPingingSeekerId = requestingPlayerId;
    
    // Update hiders' locations
    updateHidersLocation(updatedHiders);
    
    // If this is the player who requested the ping, confirm it
    // We'll compare in the UI since we don't have context here
    pingState = PingState.cooldown;
    
    notifyListeners();
  }
  
  // Handle ping rejected
  void handlePingRejected(String reason) {
    // Reset ping state
    pingState = PingState.cooldown;
    isPingRejected = true;
    pingRejectionReason = reason;
    
    // Start cooldown timer
    cooldownSeconds = 10;
    _startCooldown(() {});
    
    notifyListeners();
  }
  
  // Handle ping ended
  void handlePingEnded(String requestingPlayerId) {
    // Clear the current pinging seeker
    currentPingingSeekerId = null;
    
    // Set state to cooldown
    pingState = PingState.cooldown;
    cooldownSeconds = 10;
    
    // Start cooldown timer
    _startCooldown(() {});
    
    notifyListeners();
  }
  
  // Handle ping cooldown ended
  void handlePingCooldownEnded() {
    // Set state to idle
    pingState = PingState.idle;
    
    notifyListeners();
  }
  
  void _startCooldown(Function onCooldownComplete) {
    Future.doWhile(() async {
      await Future.delayed(const Duration(seconds: 1));
      cooldownSeconds--;
      notifyListeners();
      if (cooldownSeconds <= 0) {
        pingState = PingState.idle;
        notifyListeners();
        onCooldownComplete();
        return false;
      }
      return true;
    });
  }

  // Legacy method for backward compatibility
  void handlePingStartedFromServer() {
    pingState = PingState.pinging;
    notifyListeners();

    Future.delayed(const Duration(seconds: 5), () {
      pingState = PingState.cooldown;
      cooldownSeconds = 10;
      notifyListeners();

      _startCooldown(() {});
    });
  }
}

