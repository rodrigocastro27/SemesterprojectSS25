import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/models/ping_state.dart';
import 'package:latlong2/latlong.dart';
import 'package:geolocator/geolocator.dart'; // geolocator package

class GameState extends ChangeNotifier {
  bool isHider = false;
  PingState pingState = PingState.idle;
  int cooldownSeconds = 10;
  LatLng? userLocation;

  void setRole(bool hider) {
    isHider = hider;
    pingState = hider ? PingState.pinging : PingState.idle;
    notifyListeners();
  }

  void setLocation(LatLng location) {
    userLocation = location;
    notifyListeners();
  }

  void startPing(Function onCooldownComplete) {
    pingState = PingState.pinging;
    notifyListeners();

    Future.delayed(const Duration(seconds: 5), () {
      pingState = PingState.cooldown;
      cooldownSeconds = 10;
      notifyListeners();

      _startCooldown(onCooldownComplete);
    });
  }

  void initLocation(BuildContext context) async {
    final gameState = Provider.of<GameState>(context, listen: false);

    bool serviceEnabled = await Geolocator.isLocationServiceEnabled();
    if (!serviceEnabled) return;

    LocationPermission permission = await Geolocator.checkPermission();
    if (permission == LocationPermission.denied) {
      permission = await Geolocator.requestPermission();
    }
    if (permission == LocationPermission.deniedForever || permission == LocationPermission.denied) return;

    final position = await Geolocator.getCurrentPosition(desiredAccuracy: LocationAccuracy.high);
    gameState.setLocation(LatLng(position.latitude, position.longitude));
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
}