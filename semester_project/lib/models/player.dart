import 'package:latlong2/latlong.dart';

class Player {
  final String name;
  final String role;
  final String nickname;

  LatLng? position;
  String? qrCode;

  Player({
    required this.name,
    required this.role,
    required this.nickname,
  }) {
    qrCode = name; // Set qrCode to name initially
  }

  factory Player.fromJson(Map<String, dynamic> json) {
    final player = Player(
      name: json['name'],
      role: json['role'],
      nickname: json['nickname'],
    );
    player.qrCode = json['qrCode'] ?? player.name; // Use stored value or fallback to name
    return player;
  }

  Map<String, dynamic> toJson() {
    return {
      'name': name,
      'role': role,
      'nickname': nickname,
      'qrCode': qrCode,
    };
  }
}
