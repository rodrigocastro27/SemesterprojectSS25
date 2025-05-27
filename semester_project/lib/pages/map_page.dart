import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/state/game_state.dart';
import 'package:semester_project/widgets/seeker_map_view.dart';
import 'package:semester_project/widgets/hider_map_view.dart';


class MapPage extends StatelessWidget {
  const MapPage({super.key});

  @override
  Widget build(BuildContext context) {
    final gameState = Provider.of<GameState>(context, listen: false);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Map'),
        backgroundColor: Colors.blueGrey,
        foregroundColor: Colors.white,
        shadowColor: Colors.black,
      ),
      body: gameState.isHider ? const HiderMapView() : const SeekerMapView(),
    );
  }
}
