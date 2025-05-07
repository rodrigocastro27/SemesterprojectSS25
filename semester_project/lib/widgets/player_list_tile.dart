import 'package:flutter/material.dart';
import 'package:semester_project/models/player.dart';

class PlayerListTile extends StatelessWidget {
  final Player player;

  const PlayerListTile({super.key, required this.player});

  @override
  Widget build(BuildContext context) {
    return ListTile(
      title: Text(player.name),
      subtitle: Text(player.role),
      leading: Icon(player.role == 'Seeker' ? Icons.search : Icons.hide_source),
    );
  }
}
