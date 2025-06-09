// lib/widgets/task_overlay.dart
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/state/game_state.dart';
import 'package:semester_project/widgets/tasks/clicking_race_task.dart';

class TaskOverlay extends StatelessWidget {
  const TaskOverlay({super.key});

  @override
  Widget build(BuildContext context) {
    final gameState = Provider.of<GameState>(context);

    final taskName = gameState.currentTaskName;
    final payload = gameState.currentTaskPayload;

    if (taskName == null) return const SizedBox.shrink();

    Widget? taskWidget = _buildTaskWidget(taskName, payload ?? {});

    return taskWidget;
  }

  Widget _buildTaskWidget(String taskName, Map<String, dynamic>? payload) {
    switch (taskName) {
      case 'ClickingRace':
        return ClickingRaceTaskWidget(payload: payload ?? {});
      // Add more cases as you implement new tasks




      
      default:
        return _buildFallback(taskName);
    }
  }

  Widget _buildFallback(String taskName) {
    return Center(
      child: Container(
        padding: const EdgeInsets.all(16),
        color: Colors.black87,
        child: Text(
          "Unknown task: $taskName",
          style: const TextStyle(color: Colors.white),
        ),
      ),
    );
  }
}
