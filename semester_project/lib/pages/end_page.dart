import 'package:flutter/material.dart';

class EndGamePage extends StatelessWidget {
  final String resultMessage;
  final String? additionalInfo;

  const EndGamePage({
    super.key,
    required this.resultMessage,
    this.additionalInfo,
  });

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text("Game Over")),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(resultMessage,
                style: const TextStyle(fontSize: 24, fontWeight: FontWeight.bold)),
            if (additionalInfo != null) ...[
              const SizedBox(height: 12),
              Text(additionalInfo!),
            ],
            const SizedBox(height: 24),
            ElevatedButton(
              onPressed: () {
                Navigator.of(context).pushNamedAndRemoveUntil("/home", (r) => false);
              },
              child: const Text("Return to Home"),
            ),
          ],
        ),
      ),
    );
  }
}
