// import 'package:flutter/material.dart';
// import 'new_join_lobby.dart';
// import 'new_create_lobby.dart';

// class LobbyCat extends StatelessWidget {
//   @override
//   Widget build(BuildContext context) {
//     return Scaffold(
//       backgroundColor: Color(0xffffffff),
//       appBar: AppBar(
//         elevation: 0,
//         centerTitle: false,
//         automaticallyImplyLeading: false,
//         backgroundColor: Color(0xffffffff),
//         shape: RoundedRectangleBorder(borderRadius: BorderRadius.zero),
//         leading: IconButton(
//           icon: Icon(Icons.logout, color: Color(0xff212435), size: 24),
//           onPressed: () {
//             Navigator.pop(context);
//           },
//         ),
//       ),
//       body: Padding(
//         padding: EdgeInsets.all(16),
//         child: Column(
//           mainAxisAlignment: MainAxisAlignment.spaceAround,
//           crossAxisAlignment: CrossAxisAlignment.center,
//           mainAxisSize: MainAxisSize.max,
//           children: [
//             Column(
//               mainAxisAlignment: MainAxisAlignment.start,
//               crossAxisAlignment: CrossAxisAlignment.center,
//               mainAxisSize: MainAxisSize.max,
//               children: [
//                 ClipRRect(
//                   borderRadius: BorderRadius.circular(16),
//                   child: Image.asset(
//                     'assets/images/cat_lobby.jpeg',
//                     height: 120,
//                     width: 120,
//                     fit: BoxFit.cover,
//                   ),
//                 ),
//               ],
//             ),
//             Column(
//               mainAxisAlignment: MainAxisAlignment.start,
//               crossAxisAlignment: CrossAxisAlignment.center,
//               mainAxisSize: MainAxisSize.max,
//               children: [
//                 MaterialButton(
//                   onPressed: () {
//                     Navigator.push(
//                       context,
//                       MaterialPageRoute(
//                         builder: (context) => const CreateLobbyPage(),
//                       ),
//                     );
//                   },
//                   color: Color(0xff36c8bb),
//                   elevation: 0,
//                   shape: RoundedRectangleBorder(
//                     borderRadius: BorderRadius.circular(22.0),
//                   ),
//                   padding: EdgeInsets.all(16),
//                   child: Text(
//                     "Create Lobby",
//                     style: TextStyle(
//                       fontSize: 16,
//                       fontWeight: FontWeight.w500,
//                       fontStyle: FontStyle.normal,
//                     ),
//                   ),
//                   textColor: Color(0xffffffff),
//                   height: 50,
//                   minWidth: MediaQuery.of(context).size.width,
//                 ),
//                 Padding(
//                   padding: EdgeInsets.fromLTRB(0, 30, 0, 0),
//                   child: MaterialButton(
//                     onPressed: () {
//                       Navigator.push(
//                         context,
//                         MaterialPageRoute(
//                           builder: (context) => const JoinLobbyPage(),
//                         ),
//                       );
//                     },
//                     color: Color(0xffffffff),
//                     elevation: 0,
//                     shape: RoundedRectangleBorder(
//                       borderRadius: BorderRadius.circular(22.0),
//                       side: BorderSide(color: Color(0xff0c9c90), width: 2),
//                     ),
//                     padding: EdgeInsets.all(16),
//                     child: Text(
//                       "Join Lobby",
//                       style: TextStyle(
//                         fontSize: 16,
//                         fontWeight: FontWeight.w500,
//                         fontStyle: FontStyle.normal,
//                       ),
//                     ),
//                     textColor: Color(0xff000000),
//                     height: 50,
//                     minWidth: MediaQuery.of(context).size.width,
//                   ),
//                 ),
//               ],
//             ),
//           ],
//         ),
//       ),
//     );
//   }
// }
