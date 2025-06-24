// import 'package:flutter/material.dart';
// import 'package:semester_project/logic/message_sender.dart';
// import 'package:uuid/uuid.dart';

// class AuthPage extends StatefulWidget {
//   const AuthPage ({super.key});

//   @override
//   State<AuthPage> createState() => _AuthPageState();
// }

// class _AuthPageState extends State<AuthPage> {
//   final TextEditingController _usernameController = TextEditingController();

//   void _login() {
//     final username = _usernameController.text.trim();
//     if (username.isNotEmpty) {
      
//       // Use uuid for now, can use Firebase ID later
//       String deviceId = Uuid().v1();

//       MessageSender.loginPlayer(deviceId, username);

//       ScaffoldMessenger.of(context).showSnackBar(
//         SnackBar(content: Text('Logged in as $username')),
//       );
//     } else {
//       ScaffoldMessenger.of(context).showSnackBar(
//         SnackBar(content: Text('Please enter a username')),
//       );
//     }
//   }

//   @override
//   Widget build(BuildContext context) {
//     return Scaffold(
//       appBar: AppBar(
//         title: Text('Player Login'),
//       ),
//       body: Padding(
//         padding: const EdgeInsets.all(24.0),
//         child: Column(
//           mainAxisAlignment: MainAxisAlignment.center,
//           children: [
//             TextField(
//               controller: _usernameController,
//               decoration: InputDecoration(
//                 labelText: 'Username',
//                 border: OutlineInputBorder(),
//               ),
//             ),
//             SizedBox(height: 20),
//             ElevatedButton(
//               onPressed: _login,
//               child: Text('Log in'),
//               style: ElevatedButton.styleFrom(
//                 minimumSize: Size(double.infinity, 48),
//               ),
//             ),
//           ],
//         ),
//       ),
//     );
//   }

//   @override
//   void dispose() {
//     _usernameController.dispose();
//     super.dispose();
//   }
// }





import 'package:flutter/material.dart';
import 'package:uuid/uuid.dart';
import 'package:semester_project/logic/message_sender.dart';
import 'package:semester_project/pages/lobby pages/lobby_page.dart';

class AuthPage extends StatefulWidget {
  const AuthPage({super.key});

  @override
  _AuthPageState createState() => _AuthPageState();
}

class _AuthPageState extends State<AuthPage> {
  final TextEditingController _usernameController = TextEditingController();

  void _login() {
    final username = _usernameController.text.trim();

    if (username.isNotEmpty) {
      final deviceId = const Uuid().v1();

      MessageSender.loginPlayer(deviceId, username);

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Logged in as $username')),
      );

      // ✅ FIXED: Use push instead of pushReplacement to avoid error
      Navigator.of(context).push(
        MaterialPageRoute(builder: (context) => const LobbyPage()),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Please enter a username')),
      );
    }
  }

  @override
  void dispose() {
    _usernameController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xffffffff),
      body: Align(
        alignment: Alignment.centerLeft,
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 16),
          child: SingleChildScrollView(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.start,
              crossAxisAlignment: CrossAxisAlignment.center,
              mainAxisSize: MainAxisSize.min,
              children: [
                const Text(
                  "Meow-velous!",
                  style: TextStyle(
                    fontWeight: FontWeight.w700,
                    fontSize: 22,
                    color: Color(0xff000000),
                  ),
                ),
                const Padding(
                  padding: EdgeInsets.only(top: 8),
                  child: Text(
                    "Login to Continue",
                    style: TextStyle(
                      fontWeight: FontWeight.w400,
                      fontSize: 18,
                      color: Color(0xffa29b9b),
                    ),
                  ),
                ),
                Padding(
                  padding: const EdgeInsets.symmetric(vertical: 16),
                  child: ClipRRect(
                    borderRadius: BorderRadius.circular(16),
                    child: Image.asset(
                      'assets/images/app_icon.png',
                      height: 120,
                      width: 120,
                      fit: BoxFit.contain,
                    ),
                  ),
                ),
                TextField(
                  controller: _usernameController,
                  obscureText: false,
                  style: const TextStyle(
                    fontWeight: FontWeight.w700,
                    fontSize: 14,
                    color: Color(0xff000000),
                  ),
                  decoration: const InputDecoration(
                    labelText: "Username",
                    labelStyle: TextStyle(
                      fontWeight: FontWeight.w400,
                      fontSize: 16,
                      color: Color(0xff7c7878),
                    ),
                    enabledBorder: UnderlineInputBorder(
                      borderSide: BorderSide(color: Color(0xff000000)),
                    ),
                    focusedBorder: UnderlineInputBorder(
                      borderSide: BorderSide(color: Color(0xff000000)),
                    ),
                    filled: true,
                    fillColor: Colors.transparent,
                    isDense: false,
                    contentPadding: EdgeInsets.all(0),
                  ),
                ),
                Padding(
                  padding: const EdgeInsets.fromLTRB(0, 30, 0, 20),
                  child: MaterialButton(
                    onPressed: _login,
                    color: const Color(0xff36c8bb),
                    elevation: 0,
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(22.0),
                    ),
                    padding: const EdgeInsets.all(16),
                    child: const Text(
                      "Login",
                      style: TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                    textColor: const Color(0xffffffff),
                    height: 50,
                    minWidth: MediaQuery.of(context).size.width,
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
