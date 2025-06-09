// lib/widgets/tasks/find_location_task.dart
import 'package:flutter/material.dart';

class ClickingRaceTaskWidget extends StatelessWidget {
  final Map<String, dynamic> payload;

  const ClickingRaceTaskWidget({super.key, required this.payload});

  @override
  Widget build(BuildContext context) {
    return Positioned.fill(
      child: Container(
        color: Colors.black.withOpacity(0.7),
        child: Center(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(
                payload['description'] ?? 'Send your location',
                style: const TextStyle(color: Colors.white, fontSize: 18),
                textAlign: TextAlign.center,
              ),
              const SizedBox(height: 20),
              ElevatedButton(
                onPressed: () {
                  // Trigger location sending logic
                  print("Sending location...");
                },
                child: const Text("Send Location"),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
