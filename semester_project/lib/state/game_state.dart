import 'dart:async';
import 'package:flutter/material.dart';
import 'package:latlong2/latlong.dart';
import 'package:geolocator/geolocator.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/logic/message_sender.dart';
import 'package:semester_project/models/ping_state.dart';
import 'package:semester_project/models/player.dart';
import 'package:semester_project/state/lobby_state.dart';
import 'package:semester_project/state/player_state.dart';

class GameState extends ChangeNotifier {
  bool isHider = false;
  PingState pingState = PingState.idle;
  int cooldownSeconds = 10;
  LatLng? userLocation;
  int locationUpdateIntervalSeconds = 2;

  bool gameEnded = false;

  List<Player> hiders = [];
  List<Player> seekers = [];
  List<Player> players = [];

  // Tasks fields
  String? currentTaskName;
  Map<String,dynamic>? currentTaskPayload;
  bool startFinishingTask = false;
  String? taskResult;

  // Countdown timer fields
  DateTime? _endTime;
  Duration remainingTime = Duration.zero;
  Timer? _countdownTimer;
  Timer? _locationUpdateTimer;

  void initGame(BuildContext context) {
   
    hiders.clear();
    seekers.clear();
    players = Provider.of<LobbyState>(context, listen: false).getPlayerList();
    startLocationUpdates(context); // start updating location every 10s

    for (var p in players) {
      if (p.role == "hider") {
        hiders.add(p);
      } else if (p.role == "seeker") {
        seekers.add(p);
      }
    }

    endTask();

    print("🧑 Total players: ${players.length}");
    print("🎭 Hiders: ${hiders.map((p) => p.name).toList()}");
    print("🔍 Seekers: ${seekers.map((p) => p.name).toList()}");
  }

  void startLocationUpdates(BuildContext context) {
    // Cancel if already running
    _locationUpdateTimer?.cancel();

    // Start a new timer
    _locationUpdateTimer = Timer.periodic(
      Duration(seconds: locationUpdateIntervalSeconds),
      (_) {updatePosition(context); print("New position: (${userLocation!.latitude},${userLocation!.longitude})"); },
    );
    notifyListeners();

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

  void startPing(BuildContext context) {
    pingState = PingState.pinging;
    notifyListeners();

    final username = Provider.of<PlayerState>(context, listen: false).getUsername();
    final lobbyId = Provider.of<LobbyState>(context, listen: false).getLobbyId();

    if (lobbyId != null && username != null) {
      MessageSender.pingRequest(username, lobbyId);
    }
  }

  void initLocation(BuildContext context, String lobbyId) async {
    bool serviceEnabled = await Geolocator.isLocationServiceEnabled();
    if (!serviceEnabled) return;

    LocationPermission permission = await Geolocator.checkPermission();
    if (permission == LocationPermission.denied) {
      permission = await Geolocator.requestPermission();
    }
    if (permission == LocationPermission.deniedForever || permission == LocationPermission.denied) return;

    final position = await Geolocator.getCurrentPosition(
      desiredAccuracy: LocationAccuracy.high,
    );
    setLocation(LatLng(position.latitude, position.longitude));

    // Sent the location to the server
    MessageSender.setMapCenter(lobbyId, position.latitude, position.longitude);
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

  void updateHidersLocation(List<Player> newList) {
    hiders.clear();
    hiders = newList;
    notifyListeners();
  }

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

    void updateGameTimer(Duration duration, DateTime serverTime) {
    
    final clientTime = DateTime.now();
    final serverToClientOffset = clientTime.difference(serverTime);

    _endTime = clientTime.add(duration - serverToClientOffset);
    notifyListeners();

    print("🕒 Client Time: $clientTime");
    print("🕒 Server Time: $serverTime");
    print("🕒 Calculated End Time: $_endTime");


    _countdownTimer?.cancel(); // cancel old timer if exists
   _countdownTimer = Timer.periodic(const Duration(seconds: 1), (timer) {
    final now = DateTime.now();
    
    if (_endTime != null) {
      remainingTime = _endTime!.difference(now);

      if (remainingTime.isNegative) {
        remainingTime = Duration.zero;
        timer.cancel();
      }
      notifyListeners();
    }
  });
  }


  void stopGame() 
  {
    //additionally clean game logic...
    gameEnded = true;
    endTask();
    notifyListeners();
  }

  void reset() {
    gameEnded = false;
    // reset any other fields if needed
    notifyListeners();
  }

  void startTask(String name) {
    print("changing state");
    currentTaskName = name;
    notifyListeners();
  }

  void updatePayload(Map<String,dynamic> payload) {
    currentTaskPayload = payload;
    notifyListeners();
  }

  void finishTask(bool finish) {
    startFinishingTask = finish;
    notifyListeners();
  }

  void setTaskResult(String winners) {
    taskResult = winners;
    notifyListeners();

    Future.delayed(const Duration(seconds: 3), () {
      taskResult = null;
      notifyListeners();
    });
  }
  // More task functions
  // ...

  void endTask() {
    currentTaskName = null;
    notifyListeners();
  }


  @override
  void dispose() {
    _countdownTimer?.cancel();
    _locationUpdateTimer?.cancel();
    super.dispose();
  }
}
