import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:semester_project/models/player.dart';

class FirestoreService {
  final FirebaseFirestore _db = FirebaseFirestore.instance;

  // Create a new lobby and add a player
  Future<DocumentReference> createLobby(String lobbyName, Player creator) async {
    try {
      return await _db.collection('lobbies').add({
        'lobbyName': lobbyName,
        'members': [
          creator.toJson(),
        ],
      });
    } catch (e) {
      throw Exception("Error creating lobby: $e");
    }
  }

  // Join an existing lobby
  Future<DocumentReference?> joinLobby(String lobbyName, Player player) async {
    try {
      final query = await _db
          .collection('lobbies')
          .where('lobbyName', isEqualTo: lobbyName)
          .limit(1)
          .get();

      if (query.docs.isNotEmpty) {
        final lobbyDoc = query.docs.first.reference;
        await lobbyDoc.update({
          'members': FieldValue.arrayUnion([player.toJson()]),
        });
        return lobbyDoc;
      }
    } catch (e) {
      throw Exception("Error joining lobby: $e");
    }
    return null;
  }

  // Stream the lobby data (updates as players join/leave)
  Stream<DocumentSnapshot> lobbyStream(DocumentReference lobbyRef) {
    return lobbyRef.snapshots();
  }
}
