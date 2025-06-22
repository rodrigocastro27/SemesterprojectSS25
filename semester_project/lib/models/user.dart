class User {
  final int userId;
  final String email;
  final String token;

  User({required this.userId, required this.email, required this.token});

  factory User.fromJson(Map<String, dynamic> json) {
    return User(
      userId: json['userId'] as int,
      email: json['email'] as String,
      token: json['token'] as String,
    );
  }

  Map<String, dynamic> toJson() {
    return {'userId': userId, 'email': email, 'token': token};
  }

  @override
  String toString() {
    final displayToken =
        token.length > 20
            ? '${token.substring(0, 10)}...${token.substring(token.length - 10)}'
            : token;
    return 'User(userId: $userId, email: $email, token: $displayToken)';
  }
}
