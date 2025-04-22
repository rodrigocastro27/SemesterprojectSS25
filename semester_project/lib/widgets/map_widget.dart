import 'dart:async'; // Timer, Future
import 'package:flutter/material.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:geolocator/geolocator.dart'; // geolocator package
import 'package:latlong2/latlong.dart'; // LatLng class
import 'ping_button.dart';
import 'user_marker.dart';
import '../models/ping_state.dart';

class MapWidget extends StatefulWidget {
  const MapWidget({super.key});

  @override
  State<MapWidget> createState() => _MapWidgetState();
}

class _MapWidgetState extends State<MapWidget> {
  static const int pingDuration = 5; // time that ping is displayed in the map
  static const int cooldownDuration =
      10; // time after the ping finishes to wait to ping again

  PingState pingState = PingState.idle; // idle, pinging, cooldown
  int cooldownSeconds =
      cooldownDuration; // remaining seconds for the cooldown phase

  LatLng? userLocation; // current location of the user
  Timer? _pingTimer;
  Timer? _cooldownTimer;

  @override
  void initState() {
    super.initState(); // called once when the widget is created
    _initLocation(); // initializes user's location and handles permissions
  }

  Future<void> _initLocation() async {
    bool serviceEnabled = await Geolocator.isLocationServiceEnabled();
    if (!serviceEnabled) {
      return;
    }

    LocationPermission permission = await Geolocator.checkPermission();
    if (permission == LocationPermission.denied) {
      permission = await Geolocator.requestPermission();
    }
    if (permission == LocationPermission.deniedForever ||
        permission == LocationPermission.denied) {
      return;
    }

    Position position = await Geolocator.getCurrentPosition(
      desiredAccuracy: LocationAccuracy.high,
    );

    setState(() {
      userLocation = LatLng(position.latitude, position.longitude);
    });
  }

  void startPing() {
    // triggered when the user clicks the "Ping" button
    if (userLocation == null) return;

    _pingTimer?.cancel();
    _cooldownTimer?.cancel();

    setState(() {
      pingState = PingState.pinging;
    });

    _pingTimer = Timer(const Duration(seconds: pingDuration), () {
      setState(() {
        pingState = PingState.cooldown;
        cooldownSeconds = cooldownDuration;
      });

      _cooldownTimer = Timer.periodic(const Duration(seconds: 1), (timer) {
        setState(() {
          cooldownSeconds--;
          if (cooldownSeconds <= 0) {
            pingState = PingState.idle;
            timer.cancel();
          }
        });
      });
    });
  }

  @override
  void dispose() {
    // called when the widget is removed from the widget tree
    // make sure that timers are cleaned up
    _pingTimer?.cancel();
    _cooldownTimer?.cancel();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    if (userLocation == null) {
      return const Center(child: CircularProgressIndicator());
    }

    return Stack(
      children: [
        FlutterMap(
          options: MapOptions(
            initialCenter: userLocation!,
            initialZoom: 16,
            interactionOptions: const InteractionOptions(
              flags: ~InteractiveFlag.doubleTapZoom,
            ),
          ),
          children: [
            TileLayer(
              urlTemplate: 'https://tile.openstreetmap.org/{z}/{x}/{y}.png',
              userAgentPackageName: 'dev.fleaflet.flutter_map.example',
            ),
            MarkerLayer(
              markers: [
                if (pingState ==
                    PingState
                        .pinging) // show only when the pinging state is "pinging"
                  Marker(
                    point: userLocation!,
                    width: 28,
                    height: 28,
                    alignment: Alignment.centerLeft,
                    child: const UserMarker(),
                  ),
              ],
            ),
          ],
        ),
        Positioned(
          bottom: 20,
          left: 20,
          child: PingButton(
            onPing: startPing, // call to function to start Pinging
            pingState: pingState,
            cooldownSeconds: cooldownSeconds,
          ),
        ),
      ],
    );
  }
}
