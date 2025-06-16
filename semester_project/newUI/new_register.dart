import 'package:flutter/material.dart';
import 'new_login.dart';

class RegisterCat extends StatelessWidget {
  const RegisterCat({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xffffffff),
      appBar: AppBar(
        elevation: 0,
        centerTitle: false,
        backgroundColor: const Color(0xffffffff),
        leading: IconButton(
          icon: const Icon(Icons.arrow_back, color: Color(0xff212435)),
          onPressed: () {
            Navigator.pop(context);
          },
        ),
      ),
      body: Padding(
        padding: const EdgeInsets.symmetric(vertical: 2, horizontal: 16),
        child: SingleChildScrollView(
          child: Column(
            children: [
              const SizedBox(height: 16),
              ClipRRect(
                borderRadius: BorderRadius.circular(16),
                child: const Image(
                  image: AssetImage('assets/images/test-image.jpeg'),
                  height: 100,
                  width: 100,
                  fit: BoxFit.cover,
                ),
              ),
              const SizedBox(height: 16),
              const Text(
                "Purr-fect!",
                style: TextStyle(
                  fontWeight: FontWeight.w700,
                  fontSize: 22,
                  color: Color(0xff36c8bb),
                ),
              ),
              const SizedBox(height: 8),
              const Text(
                "Create an account",
                style: TextStyle(fontSize: 14, color: Colors.black),
              ),
              const SizedBox(height: 20),
              _buildTextField("Name", Icons.person),
              _buildTextField("Email Address", Icons.mail),
              _buildTextField("Password", Icons.lock, obscure: true),
              _buildTextField(
                "Confirm Password",
                Icons.lock_outline,
                obscure: true,
              ),
              const SizedBox(height: 16),
              MaterialButton(
                onPressed: () {
                },
                color: const Color(0xff36c8bb),
                elevation: 0,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(22.0),
                ),
                padding: const EdgeInsets.all(16),
                child: const Text(
                  "Register",
                  style: TextStyle(fontSize: 16, fontWeight: FontWeight.w700),
                ),
                textColor: Colors.white,
                height: 45,
                minWidth: MediaQuery.of(context).size.width,
              ),
              const SizedBox(height: 16),
              Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const Text(
                    "Already have an account?",
                    style: TextStyle(fontSize: 14),
                  ),
                  const SizedBox(width: 8),
                  GestureDetector(
                    onTap: () {
                      Navigator.pop(context); 
                    },
                    child: const Text(
                      "Sign In",
                      style: TextStyle(
                        fontWeight: FontWeight.bold,
                        fontSize: 14,
                        color: Color(0xff36c8bb),
                      ),
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildTextField(String hint, IconData icon, {bool obscure = false}) {
    return Padding(
      padding: const EdgeInsets.only(top: 16),
      child: TextField(
        obscureText: obscure,
        decoration: InputDecoration(
          hintText: hint,
          prefixIcon: Icon(icon, color: const Color(0xff212435)),
          border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(8.0),
            borderSide: const BorderSide(color: Color(0xff0c9c90), width: 1),
          ),
          enabledBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(8.0),
            borderSide: const BorderSide(color: Color(0xff0c9c90), width: 1),
          ),
          focusedBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(8.0),
            borderSide: const BorderSide(color: Color(0xff0c9c90), width: 1),
          ),
        ),
      ),
    );
  }
}