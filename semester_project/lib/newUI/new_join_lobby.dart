// import 'package:flutter/material.dart';
// import 'package:semester_project/models/player.dart';
// import 'package:semester_project/pages/lobby pages/lobby_page.dart'; // <-- Use your old lobby page
// import 'package:semester_project/services/firestore_services.dart';

// class JoinLobbyPage extends StatefulWidget {
//   const JoinLobbyPage({super.key});

//   @override
//   State<JoinLobbyPage> createState() => _JoinLobbyPageState();
// }

// class _JoinLobbyPageState extends State<JoinLobbyPage> {
//   final TextEditingController _lobbyNameController = TextEditingController();
//   final TextEditingController _usernameController = TextEditingController();
//   String _selectedRole = 'Hider';

//   void _joinLobby() async {
//     final lobbyName = _lobbyNameController.text.trim();
//     final username = _usernameController.text.trim();
//     if (lobbyName.isEmpty || username.isEmpty) return;

//     final player = Player(name: username, role: _selectedRole);

//     final firestoreService = FirestoreService();
//     final lobbyDoc = await firestoreService.joinLobby(lobbyName, player);

//     if (lobbyDoc != null) {
//       Navigator.push(
//         context,
//         MaterialPageRoute(builder: (_) => LobbyPage(lobbyRef: lobbyDoc)),
//       );
//     } else {
//       ScaffoldMessenger.of(context).showSnackBar(
//         const SnackBar(content: Text('Lobby not found')),
//       );
//     }
//   }

//   @override
//   Widget build(BuildContext context) {
//     return Scaffold(
//       backgroundColor: Color(0xffffffff),
//       appBar: AppBar(
//         elevation: 0,
//         backgroundColor: Color(0xffffffff),
//         leading: IconButton(
//           icon: const Icon(Icons.arrow_back, color: Color(0xff212435)),
//           onPressed: () => Navigator.pop(context),
//         ),
//         title: const Text(
//           "Join Lobby",
//           style: TextStyle(color: Colors.black, fontWeight: FontWeight.bold),
//         ),
//       ),
//       body: SingleChildScrollView(
//         padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 16),
//         child: Column(
//           crossAxisAlignment: CrossAxisAlignment.center,
//           children: [
//             ClipRRect(
//               borderRadius: BorderRadius.circular(16),
//               child: Image.asset(
//                 'assets/images/bingus.png',
//                 height: 120,
//                 width: 120,
//                 fit: BoxFit.cover,
//               ),
//             ),
//             const SizedBox(height: 24),
//             TextField(
//               controller: _lobbyNameController,
//               decoration: InputDecoration(
//                 labelText: "Lobby Name",
//                 labelStyle: TextStyle(color: Color(0xff7c7878)),
//                 enabledBorder: UnderlineInputBorder(
//                   borderSide: BorderSide(color: Color(0xff0c9c90)),
//                 ),
//                 focusedBorder: UnderlineInputBorder(
//                   borderSide: BorderSide(color: Color(0xff0c9c90)),
//                 ),
//               ),
//             ),
//             const SizedBox(height: 20),
//             TextField(
//               controller: _usernameController,
//               decoration: InputDecoration(
//                 labelText: "Username",
//                 labelStyle: TextStyle(color: Color(0xff7c7878)),
//                 enabledBorder: UnderlineInputBorder(
//                   borderSide: BorderSide(color: Color(0xff0c9c90)),
//                 ),
//                 focusedBorder: UnderlineInputBorder(
//                   borderSide: BorderSide(color: Color(0xff0c9c90)),
//                 ),
//               ),
//             ),
//             const SizedBox(height: 20),
//             DropdownButtonFormField<String>(
//               value: _selectedRole,
//               decoration: InputDecoration(
//                 labelText: "Role",
//                 labelStyle: TextStyle(color: Color(0xff7c7878)),
//                 enabledBorder: UnderlineInputBorder(
//                   borderSide: BorderSide(color: Color(0xff0c9c90)),
//                 ),
//                 focusedBorder: UnderlineInputBorder(
//                   borderSide: BorderSide(color: Color(0xff0c9c90)),
//                 ),
//               ),
//               onChanged: (value) {
//                 setState(() {
//                   _selectedRole = value!;
//                 });
//               },
//               items: ['Hider', 'Seeker']
//                   .map(
//                     (role) => DropdownMenuItem(
//                       value: role,
//                       child: Text(
//                         role,
//                         style: TextStyle(color: Colors.black),
//                       ),
//                     ),
//                   )
//                   .toList(),
//             ),
//             const SizedBox(height: 40),
//             MaterialButton(
//               onPressed: _joinLobby,
//               color: Color(0xff36c8bb),
//               elevation: 0,
//               shape: RoundedRectangleBorder(
//                 borderRadius: BorderRadius.circular(22.0),
//               ),
//               padding: const EdgeInsets.all(16),
//               child: const Text(
//                 "Join Lobby",
//                 style: TextStyle(
//                   fontSize: 16,
//                   fontWeight: FontWeight.w700,
//                   color: Colors.white,
//                 ),
//               ),
//               height: 50,
//               minWidth: double.infinity,
//             ),
//           ],
//         ),
//       ),
//     );
//   }
// }
