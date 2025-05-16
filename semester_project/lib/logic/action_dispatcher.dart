import 'dart:convert';

typedef ServerActionHandler = void Function(Map<String, dynamic> data);

class ServerActionDispatcher {
  final Map<String, ServerActionHandler> _handlers = {};

  void register(String action, ServerActionHandler handler) {
    _handlers[action] = handler;
  }

  void handleMessage(String message) {
    final parsed = jsonDecode(message);
    final action = parsed['action'];
    final data = parsed['data'] ?? {};

    if (_handlers.containsKey(action)) {
      _handlers[action]!(data);
    } else {
      print("Unhandled action: $action");
    }
  }
}
