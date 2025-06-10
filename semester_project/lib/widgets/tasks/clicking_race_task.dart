import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/logic/message_sender.dart';
import 'package:semester_project/state/game_state.dart';
import 'package:semester_project/state/player_state.dart';
import 'package:semester_project/state/lobby_state.dart';


class ClickingRaceTaskWidget extends StatefulWidget {
  const ClickingRaceTaskWidget({super.key});

  @override
  State<ClickingRaceTaskWidget> createState() => ClickingRaceTaskWidgetState();
}

class ClickingRaceTaskWidgetState extends State<ClickingRaceTaskWidget>
    with SingleTickerProviderStateMixin {
  int clickCount = 0;
  double _scale = 1.0;
  bool taskFinished = false;

  @override
  void initState() {
    super.initState();
    _monitorTaskEnd();
  }

  void increment() {
    if (!taskFinished) {
      setState(() {
        clickCount++;
        _scale = 1.1;
      });
    }

    // Then scale back down after a short delay
    Future.delayed(const Duration(milliseconds: 100), () {
      if (mounted) {
        setState(() {
          _scale = 1.0;
        });
      }
    });
  }

  Future<void> _monitorTaskEnd() async {
    while (mounted && !taskFinished) {
      await Future.delayed(const Duration(milliseconds: 300)); // Polling interval
      var startFinishing = Provider.of<GameState>(context, listen:false).startFinishingTask;

      if (startFinishing) {
        _finishTask();
      }
    }
  }

  void _finishTask() {
    if (!mounted || taskFinished) return;

    setState(() {
      taskFinished = true;
    });

    final username = context.read<PlayerState>().getUsername();
    final lobbyId = context.read<LobbyState>().getLobbyId();
    final isHider = context.read<GameState>().isHider;

    // Show message
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text('Task finished with $clickCount clicks')),
    );

    // Send message to server
    if (username != null && lobbyId != null) {
      Map<String,dynamic> payload = {
        'count' : clickCount,
        'isHider' : isHider,
      };
      Map<String, dynamic> update = {
        'taskName': 'ClickingRace',
        'update' : {
          'type' : 'finishedCounting',
          'info' : payload,
        }
      };
      Provider.of<GameState>(context, listen:false).finishTask(false);
      MessageSender.sendTaskUpdate(lobbyId, username, update);
    }

    // Optionally: clear the task
    context.read<GameState>().endTask();
  }

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Container(
        margin: const EdgeInsets.all(12),
        padding: const EdgeInsets.all(32),
        decoration: BoxDecoration(
          color: Colors.black.withOpacity(0.7),
          borderRadius: BorderRadius.circular(16),
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Text(
              'Click Me(ow)!',
              style: TextStyle(
                color: Colors.white,
                fontSize: 22,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            const Text(
              'Click as many times as you can before time runs out!',
              style: TextStyle(color: Colors.white, fontSize: 18),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 16),
            Text(
              'Clicks: $clickCount',
              style: const TextStyle(color: Colors.white, fontSize: 20),
            ),
            const SizedBox(height: 30),
            AnimatedScale(
              scale: _scale,
              duration: const Duration(milliseconds: 100),
              child: ElevatedButton.icon(
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.amber,
                  foregroundColor: Colors.black,
                  padding:
                      const EdgeInsets.symmetric(horizontal: 24, vertical: 20),
                  textStyle:
                      const TextStyle(fontSize: 22, fontWeight: FontWeight.bold),
                  elevation: 12,
                  shadowColor: Colors.amberAccent,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(30),
                  ),
                ),
                onPressed: increment,
                icon: const Icon(Icons.pets, size: 28),
                label: const Text('CLICK ME!'),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
