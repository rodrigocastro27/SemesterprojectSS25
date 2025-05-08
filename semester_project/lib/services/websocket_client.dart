import 'package:web_socket_channel/web_socket_channel.dart';

class WebSocketService {
  final WebSocketChannel channel;

  WebSocketService._(this.channel);

  static WebSocketService connect(String url) {
    final channel = WebSocketChannel.connect(Uri.parse(url));
    return WebSocketService._(channel);
  }

  void send(String message) {
    channel.sink.add(message);
  }

  Stream get stream => channel.stream;

  void disconnect() {
    channel.sink.close();
  }
}


