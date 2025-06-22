import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:semester_project/logic/message_sender.dart'; 
import 'package:semester_project/state/player_state.dart'; 


class AuthenticationPage extends StatefulWidget {
  
  const AuthenticationPage({super.key}); 

  @override
  State<AuthenticationPage> createState() => _AuthenticationPageState(); 
}

class _AuthenticationPageState extends State<AuthenticationPage> {
  
  
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  final _confirmPasswordController = TextEditingController();
  bool _isRegistering = false;

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    _confirmPasswordController.dispose();

    super.dispose();
  }

  
  void _submitAuthForm() {
    final email = _emailController.text.trim();
    final password = _passwordController.text.trim();
    final confirmPassword =
        _confirmPasswordController.text.trim(); 

    if (email.isEmpty || password.isEmpty) {
      _showSnackBar('Email and password cannot be empty.', Colors.red);
      return;
    }

    if (_isRegistering) {
      if (password != confirmPassword) {
        _showSnackBar('Passwords do not match.', Colors.red);
        return;
      }
     
      MessageSender.register(
        email: email,
        password: password,
      ); 
      _showSnackBar('Attempting to register...', Colors.blue);
    } else {
     
      MessageSender.login(
        email: email,
        password: password,
      ); 
      _showSnackBar('Attempting to log in...', Colors.blue);
    }
  }

 
  void _showSnackBar(String message, Color color) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: color,
        duration: const Duration(
          seconds: 2,
        ), 
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
   
    final playerState = Provider.of<PlayerState>(context);

   
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (playerState.authenticatedUser != null) {
        
        Navigator.of(context).pushReplacementNamed('/lobby');
      
      }
    });

    return Scaffold(
      appBar: AppBar(
        
        title: Text(_isRegistering ? 'Register Account' : 'Login to Account'),
        backgroundColor: Colors.blueAccent,
      ),
      body: Center(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(24.0),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: <Widget>[
              
              TextField(
                controller: _emailController,
                keyboardType: TextInputType.emailAddress,
                decoration: InputDecoration(
                  labelText: 'Email',
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(10),
                  ),
                  prefixIcon: const Icon(Icons.email),
                ),
              ),
              const SizedBox(height: 16), 
             
              TextField(
                controller: _passwordController,
                obscureText: true,
                decoration: InputDecoration(
                  labelText: 'Password',
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(10),
                  ),
                  prefixIcon: const Icon(Icons.lock),
                ),
              ),
              const SizedBox(height: 16),

              if (_isRegistering) 
                TextField(
                  controller: _confirmPasswordController,
                  obscureText: true,
                  decoration: InputDecoration(
                    labelText: 'Confirm Password',
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(10),
                    ),
                    prefixIcon: const Icon(Icons.lock_reset),
                  ),
                ),
              if (_isRegistering)
                const SizedBox(height: 16),
              // Submit Button
              ElevatedButton(
                onPressed: _submitAuthForm, 
                style: ElevatedButton.styleFrom(
                  padding: const EdgeInsets.symmetric(vertical: 15),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(10),
                  ),
                  backgroundColor: Colors.blueAccent,
                  foregroundColor: Colors.white,
                  elevation: 5, 
                ),
                child: Text(
                  _isRegistering ? 'Register' : 'Login',
                  style: const TextStyle(fontSize: 18),
                ),
              ),
              const SizedBox(height: 10),

              
              TextButton(
                onPressed: () {
                  setState(() {
                    _isRegistering = !_isRegistering;
                    _passwordController
                        .clear(); 
                    _confirmPasswordController.clear();
                  });
                },
                child: Text(
                  _isRegistering
                      ? 'Already have an account? Login'
                      : 'Need an account? Register',
                  style: TextStyle(color: Colors.blueGrey, fontSize: 16),
                ),
              ),
              const SizedBox(height: 20),

              
              TextButton(
                onPressed: () {
                  if (_emailController.text.trim().isEmpty) {
                    _showSnackBar(
                      'Please enter your email to reset password.',
                      Colors.orange,
                    );
                    return;
                  }
                  MessageSender.requestPasswordReset(
                    email: _emailController.text.trim(),
                  );
                  _showSnackBar(
                    'Sending password reset request...',
                    Colors.blue,
                  );
                },
                child: const Text(
                  'Forgot Password?',
                  style: TextStyle(color: Colors.blueGrey, fontSize: 16),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
