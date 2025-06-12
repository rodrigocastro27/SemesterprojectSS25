import 'dart:async';

import 'package:flutter/material.dart';
import 'package:mobile_scanner/mobile_scanner.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/logic/message_sender.dart';
import 'package:semester_project/state/game_state.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:semester_project/state/lobby_state.dart';

import 'ping_button.dart';
import 'user_marker.dart'; // Rename to HiderMarker if needed
import '../models/ping_state.dart';

class SeekerMapView extends StatefulWidget {
  const SeekerMapView({Key? key}) : super(key: key);

  @override
  State<SeekerMapView> createState() => _SeekerMapViewState();
}

class _SeekerMapViewState extends State<SeekerMapView> {
  late final MobileScannerController _scannerController;

  @override
  void initState() {
    super.initState();
    _scannerController = MobileScannerController(detectionSpeed: DetectionSpeed.noDuplicates);
  }

  @override
  void dispose() {
    _scannerController.dispose();
    super.dispose();
  }

  void showScanner(BuildContext context, GameState gameState, LobbyState lobbyState) {
   
   
    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text("Scan Hider's QR code"),
        content: SizedBox(
          width: 300,
          height: 400,
          child: MobileScanner(
            controller: _scannerController,
            onDetect: (capture) {
              final List<Barcode> barcodes = capture.barcodes;
              for (final barcode in barcodes) {
                debugPrint('Barcode found! ${barcode.rawValue}');
                String? hiderId = barcode.rawValue;

                //if(gameState.players.contains(hiderId)){
                  MessageSender.eliminatePlayer(hiderId!, lobbyState.lobbyId!);
                  print("eleminated player!!");
                //}else {print("player not found...");}
                
              }
              // Optionally close the dialog after successful scan:
               Navigator.of(ctx).pop();
            },
          ),
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final gameState = Provider.of<GameState>(context);
    final lobbyState = Provider.of<LobbyState>(context, listen: false);

    // Show loading indicator while user location is initializing
    if (gameState.userLocation == null) {
      gameState.initLocation(context);
      return const Center(child: CircularProgressIndicator());
    }

    return Stack(
      children: [
        FlutterMap(
          options: MapOptions(
            initialCenter: gameState.userLocation!,
            initialZoom: 16,
          ),
          children: [
            TileLayer(
              urlTemplate: 'https://tile.openstreetmap.org/{z}/{x}/{y}.png',
              userAgentPackageName: 'dev.fleaflet.flutter_map.example',
            ),
            MarkerLayer(
              markers: gameState.pingState == PingState.pinging
                  ? gameState.hiders
                      .where((p) => p.position != null)
                      .map(
                        (p) => Marker(
                          point: p.position!,
                          width: 28,
                          height: 28,
                          child: const UserMarker(),
                        ),
                      )
                      .toList()
                  : [],
            ),
          ],
        ),

        // Ping Button UI
        Positioned(
          bottom: 20,
          left: 20,
          child: PingButton(
            onPing: () => gameState.startPing(context),
            pingState: gameState.pingState,
            cooldownSeconds: gameState.cooldownSeconds,
          ),
        ),

        // Scan QR Code Button
        Positioned(
          bottom: 200,
          right: 20,
          child: FloatingActionButton(
            onPressed: () => showScanner(context, gameState, lobbyState),
            child: const Icon(Icons.qr_code_scanner),
          ),
        ),
      ],
    );
  }
}
