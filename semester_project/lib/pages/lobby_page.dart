import 'package:flutter/material.dart';
import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:semester_project/widgets/player_list_tile.dart';
import 'package:semester_project/models/player.dart';

class LobbyPage extends StatelessWidget {
  final DocumentReference lobbyRef;

  const LobbyPage({super.key, required this.lobbyRef});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Lobby')),
      body: StreamBuilder<DocumentSnapshot>(
        stream: lobbyRef.snapshots(),
        builder: (context, snapshot) {
          if (!snapshot.hasData) return const Center(child: CircularProgressIndicator());

          final data = snapshot.data!.data() as Map<String, dynamic>;
          final members = data['members'] as List<dynamic>;

          return ListView.builder(
            itemCount: members.length,
            itemBuilder: (context, index) {
              final playerData = members[index];
              final player = Player.fromJson(playerData);
              return PlayerListTile(player: player);
            },
          );
        },
      ),
    );
  }
}
