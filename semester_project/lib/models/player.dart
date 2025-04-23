class Player {
  final String name;
  final String role;

  Player({required this.name, required this.role});

  factory Player.fromJson(Map<String, dynamic> json) {
    return Player(
      name: json['name'],
      role: json['role'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'name': name,
      'role': role,
    };
  }
}
