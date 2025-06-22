import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/logic/action_dispatcher.dart'; 
import 'package:semester_project/models/user.dart'; 
import 'package:semester_project/state/player_state.dart'; 
import 'package:semester_project/services/navigation_service.dart'; 


class AuthenticationHandler {
  static void register(ServerActionDispatcher dispatcher, BuildContext context) {
    final playerState = Provider.of<PlayerState>(context, listen: false);
    

  
    dispatcher.register('register_response', (data) {
      final status = data['status'] as String;
      final message = data['message'] as String;

      if (status == 'success') {
        final userId = data['data']['userId'] as int;
        final email = data['data']['email'] as String;
        print("✅ Registration successful: $message (UserID: $userId, Email: $email)");
        
        _showSuccess("Registration successful! Please log in.", context);
      } else {
        print("❌ Registration failed: $message");
        _showError("Registration failed: $message");
      }
    });

  
    dispatcher.register('login_response', (data) {
      final status = data['status'] as String;
      final message = data['message'] as String;

      if (status == 'success') {
        final userData = data['data'] as Map<String, dynamic>;
        final user = User.fromJson(userData); 
        
        print("✅ Login successful: $message (UserID: ${user.userId}, Token: ${user.token.substring(0, 10)}...)");
        playerState.setAuthenticatedUser(user); 
        _showSuccess("Login successful! Welcome ${user.email}", context);
       
      } else {
        print("❌ Login failed: $message");
        _showError("Login failed: $message");
      }
    });

    
    dispatcher.register('password_reset_request_response', (data) {
      final status = data['status'] as String;
      final message = data['message'] as String;

      if (status == 'success') {
        print("✅ Password reset request successful: $message");
        _showSuccess(message, context); 
      } else {
        print("❌ Password reset request failed: $message");
        _showError("Password reset request failed: $message");
      }
    });

    // --- Password Update Response Handler ---
    dispatcher.register('password_update_response', (data) {
      final status = data['status'] as String;

      final message = data['data'] != null && data['data'].containsKey('message') 
                      ? data['data']['message'] as String 
                      : data['message'] as String;


      if (status == 'success') {
        print("✅ Password update successful: $message");
        _showSuccess(message, context);
       
      } else {
        print("❌ Password update failed: $message");
        _showError("Password update failed: $message");
      }
    });
  }

 
  static void _showError(String message) {
    final context = rootNavigatorKey.currentContext;

    if (context == null) {
      print("Context not available for error dialog: $message");
      return;
    }

    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text("Error"),
        content: Text(message),
        actions: [
          TextButton(
            onPressed: () {
              Navigator.pop(ctx);
            },
            child: const Text("OK"),
          ),
        ],
      ),
    );
  }

  
  static void _showSuccess(String message, BuildContext context) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(message)),
    );
  }
}
