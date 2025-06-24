import 'package:flutter/material.dart';
import '../models/ping_state.dart';

class PingButton extends StatelessWidget {
  final VoidCallback onPing;
  final PingState pingState;
  final int cooldownSeconds;
  final bool isEnabled;

  static const int totalCooldown = 10;

  const PingButton({
    super.key,
    required this.onPing,
    required this.pingState,
    this.cooldownSeconds = 10,
    this.isEnabled = true,
  });

  @override
  Widget build(BuildContext context) {
    final bool isDisabled = pingState != PingState.idle || !isEnabled;

    return Stack(
      alignment: Alignment.center,
      children: [
        if (pingState == PingState.cooldown)
          SizedBox(
            width: 70,
            height: 70,
            child: TweenAnimationBuilder<double>(
              duration: const Duration(seconds: totalCooldown),
              tween: Tween<double>(begin: 1.0, end: 0.0),
              curve: Curves.easeInOut,
              builder: (context, value, child) {
                return CircularProgressIndicator(
                  value: value,
                  strokeWidth: 6,
                  backgroundColor: Colors.grey[300],
                  valueColor: const AlwaysStoppedAnimation<Color>(
                    Colors.indigo,
                  ),
                );
              },
            ),
          ),
        ElevatedButton(
          onPressed: isDisabled ? null : onPing,
          style: ElevatedButton.styleFrom(
            backgroundColor: isDisabled ? Colors.grey[700] : Colors.black,
            padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 14),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(12),
            ),
          ),
          child: AnimatedSwitcher(
            duration: const Duration(milliseconds: 300),
            child: pingState == PingState.cooldown
                ? Text(
                    'Wait ($cooldownSeconds)',
                    key: const ValueKey('cooldown'),
                    style: const TextStyle(color: Colors.white, fontSize: 16),
                  )
                : const Text(
                    'Ping',
                    key: ValueKey('ping'),
                    style: TextStyle(color: Colors.white, fontSize: 18),
                  ),
          ),
        ),
      ],
    );
  }
}
