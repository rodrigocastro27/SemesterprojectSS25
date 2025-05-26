import 'package:latlong2/latlong.dart';

class Player {
  final String name;
  final String role;
  final String nickname;

  LatLng? position;

  Player({required this.name, required this.role, required this.nickname});

  factory Player.fromJson(Map<String, dynamic> json) {
    return Player(
      name: json['name'],
      role: json['role'],
      nickname: json['nickname'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'name': name,
      'role': role,
      'nickname': nickname,
    };
  }
}
