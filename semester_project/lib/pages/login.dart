// import 'package:flutter/material.dart';
// import 'register.dart';

// class LoginCat extends StatelessWidget {
//   const LoginCat({super.key});
//   @override
//   Widget build(BuildContext context) {
//     return Scaffold(
//       backgroundColor: Color(0xffffffff),
//       body: Align(
//         alignment: Alignment.centerLeft,
//         child: Padding(
//           padding: EdgeInsets.symmetric(vertical: 0, horizontal: 16),
//           child: SingleChildScrollView(
//             child: Column(
//               mainAxisAlignment: MainAxisAlignment.start,
//               crossAxisAlignment: CrossAxisAlignment.center,
//               mainAxisSize: MainAxisSize.min,
//               children: [
//                 Text(
//                   "Meow-velous!",
//                   textAlign: TextAlign.start,
//                   overflow: TextOverflow.clip,
//                   style: TextStyle(
//                     fontWeight: FontWeight.w700,
//                     fontStyle: FontStyle.normal,
//                     fontSize: 22,
//                     color: Color(0xff000000),
//                   ),
//                 ),
//                 Padding(
//                   padding: EdgeInsets.fromLTRB(0, 8, 0, 0),
//                   child: Text(
//                     "Login to Continue",
//                     textAlign: TextAlign.start,
//                     overflow: TextOverflow.clip,
//                     style: TextStyle(
//                       fontWeight: FontWeight.w400,
//                       fontStyle: FontStyle.normal,
//                       fontSize: 18,
//                       color: Color(0xffa29b9b),
//                     ),
//                   ),
//                 ),
//                 Padding(
//                   padding: EdgeInsets.symmetric(vertical: 16, horizontal: 0),
//                   child: Align(
//                     alignment: Alignment.center,
//                     child: ClipRRect(
//                       borderRadius: BorderRadius.circular(16),
//                       child: Image(
//                         image: AssetImage('assets/images/app_icon.png'),
//                         height: 120,
//                         width: 120,
//                         fit: BoxFit.contain,
//                       ),
//                     ),
//                   ),
//                 ),
//                 TextField(
//                   controller: TextEditingController(),
//                   obscureText: false,
//                   textAlign: TextAlign.start,
//                   maxLines: 1,
//                   style: TextStyle(
//                     fontWeight: FontWeight.w700,
//                     fontStyle: FontStyle.normal,
//                     fontSize: 14,
//                     color: Color(0xff000000),
//                   ),
//                   decoration: InputDecoration(
//                     disabledBorder: UnderlineInputBorder(
//                       borderRadius: BorderRadius.circular(4.0),
//                       borderSide: BorderSide(
//                         color: Color(0xff000000),
//                         width: 1,
//                       ),
//                     ),
//                     focusedBorder: UnderlineInputBorder(
//                       borderRadius: BorderRadius.circular(4.0),
//                       borderSide: BorderSide(
//                         color: Color(0xff000000),
//                         width: 1,
//                       ),
//                     ),
//                     enabledBorder: UnderlineInputBorder(
//                       borderRadius: BorderRadius.circular(4.0),
//                       borderSide: BorderSide(
//                         color: Color(0xff000000),
//                         width: 1,
//                       ),
//                     ),
//                     labelText: "Email",
//                     labelStyle: TextStyle(
//                       fontWeight: FontWeight.w400,
//                       fontStyle: FontStyle.normal,
//                       fontSize: 16,
//                       color: Color(0xff7c7878),
//                     ),
//                     // hintText: "Enter Text",
//                     // hintStyle: TextStyle(
//                     //   fontWeight: FontWeight.w400,
//                     //   fontStyle: FontStyle.normal,
//                     //   fontSize: 14,
//                     //   color: Color(0xff000000),
//                     // ),
//                     filled: true,
//                     fillColor: Color(0x00ffffff),
//                     isDense: false,
//                     contentPadding: EdgeInsets.all(0),
//                   ),
//                 ),
//                 Padding(
//                   padding: EdgeInsets.symmetric(vertical: 16, horizontal: 0),
//                   child: TextField(
//                     controller: TextEditingController(),
//                     obscureText: true,
//                     textAlign: TextAlign.start,
//                     maxLines: 1,
//                     style: TextStyle(
//                       fontWeight: FontWeight.w700,
//                       fontStyle: FontStyle.normal,
//                       fontSize: 14,
//                       color: Color(0xff000000),
//                     ),
//                     decoration: InputDecoration(
//                       disabledBorder: UnderlineInputBorder(
//                         borderRadius: BorderRadius.circular(4.0),
//                         borderSide: BorderSide(
//                           color: Color(0xff000000),
//                           width: 1,
//                         ),
//                       ),
//                       focusedBorder: UnderlineInputBorder(
//                         borderRadius: BorderRadius.circular(4.0),
//                         borderSide: BorderSide(
//                           color: Color(0xff000000),
//                           width: 1,
//                         ),
//                       ),
//                       enabledBorder: UnderlineInputBorder(
//                         borderRadius: BorderRadius.circular(4.0),
//                         borderSide: BorderSide(
//                           color: Color(0xff000000),
//                           width: 1,
//                         ),
//                       ),
//                       labelText: "Password",
//                       labelStyle: TextStyle(
//                         fontWeight: FontWeight.w400,
//                         fontStyle: FontStyle.normal,
//                         fontSize: 16,
//                         color: Color(0xff7c7878),
//                       ),
//                       // hintText: "Enter Text",
//                       // hintStyle: TextStyle(
//                       //   fontWeight: FontWeight.w400,
//                       //   fontStyle: FontStyle.normal,
//                       //   fontSize: 14,
//                       //   color: Color(0xff000000),
//                       // ),
//                       filled: true,
//                       fillColor: Color(0x00ffffff),
//                       isDense: false,
//                       contentPadding: EdgeInsets.all(0),
//                       suffixIcon: Icon(
//                         Icons.visibility,
//                         color: Color(0xff7b7c82),
//                         size: 24,
//                       ),
//                     ),
//                   ),
//                 ),
//                 Align(
//                   alignment: Alignment.centerLeft,
//                   child: Text(
//                     "Forgot Password?",
//                     textAlign: TextAlign.start,
//                     overflow: TextOverflow.clip,
//                     style: TextStyle(
//                       fontWeight: FontWeight.w700,
//                       fontStyle: FontStyle.normal,
//                       fontSize: 14,
//                       color: Color(0xff3a57e8),
//                     ),
//                   ),
//                 ),
//                 Padding(
//                   padding: EdgeInsets.fromLTRB(0, 30, 0, 20),
//                   child: MaterialButton(
//                     onPressed: () {},
//                     color: Color(0xff3a57e8),
//                     elevation: 0,
//                     shape: RoundedRectangleBorder(
//                       borderRadius: BorderRadius.circular(12.0),
//                     ),
//                     padding: EdgeInsets.all(16),
//                     child: Text(
//                       "Login",
//                       style: TextStyle(
//                         fontSize: 16,
//                         fontWeight: FontWeight.w700,
//                         fontStyle: FontStyle.normal,
//                       ),
//                     ),
//                     textColor: Color(0xffffffff),
//                     height: 50,
//                     minWidth: MediaQuery.of(context).size.width,
//                   ),
//                 ),
//                 MaterialButton(
//                   onPressed: () {
//                     Navigator.push(
//                       context,
//                       MaterialPageRoute(
//                         builder: (context) => const RegisterCat(),
//                       ),
//                     );
//                   },
//                   color: Color(0x2d3a57e8),
//                   elevation: 0,
//                   shape: RoundedRectangleBorder(
//                     borderRadius: BorderRadius.circular(12.0),
//                   ),
//                   padding: EdgeInsets.all(16),
//                   child: Text(
//                     "Register",
//                     style: TextStyle(
//                       fontSize: 16,
//                       fontWeight: FontWeight.w700,
//                       fontStyle: FontStyle.normal,
//                     ),
//                   ),
//                   textColor: Color(0xff3a57e8),
//                   height: 50,
//                   minWidth: MediaQuery.of(context).size.width,
//                 ),
//               ],
//             ),
//           ),
//         ),
//       ),
//     );
//   }
// }


import 'package:flutter/material.dart';
import 'register.dart';

class LoginCat extends StatefulWidget {
  const LoginCat({super.key});

  @override
  _LoginCatState createState() => _LoginCatState();
}

class _LoginCatState extends State<LoginCat> {
  bool _obscurePassword = true;

  // Persist controllers
  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Color(0xffffffff),
      body: Align(
        alignment: Alignment.centerLeft,
        child: Padding(
          padding: EdgeInsets.symmetric(horizontal: 16),
          child: SingleChildScrollView(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.start,
              crossAxisAlignment: CrossAxisAlignment.center,
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(
                  "Meow-velous!",
                  style: TextStyle(
                    fontWeight: FontWeight.w700,
                    fontSize: 22,
                    color: Color(0xff000000),
                  ),
                ),
                Padding(
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
                  padding: EdgeInsets.symmetric(vertical: 16),
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
                  controller: _emailController,
                  obscureText: false,
                  style: TextStyle(
                    fontWeight: FontWeight.w700,
                    fontSize: 14,
                    color: Color(0xff000000),
                  ),
                  decoration: InputDecoration(
                    labelText: "Email",
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
                  padding: EdgeInsets.symmetric(vertical: 16),
                  child: TextField(
                    controller: _passwordController,
                    obscureText: _obscurePassword,
                    style: TextStyle(
                      fontWeight: FontWeight.w700,
                      fontSize: 14,
                      color: Color(0xff000000),
                    ),
                    decoration: InputDecoration(
                      labelText: "Password",
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
                      suffixIcon: IconButton(
                        icon: Icon(
                          _obscurePassword
                              ? Icons.visibility_off
                              : Icons.visibility,
                          color: Color(0xff7b7c82),
                        ),
                        onPressed: () {
                          setState(() {
                            _obscurePassword = !_obscurePassword;
                          });
                        },
                      ),
                    ),
                  ),
                ),
                Align(
                  alignment: Alignment.centerLeft,
                  child: Text(
                    "Forgot Password?",
                    style: TextStyle(
                      fontWeight: FontWeight.w700,
                      fontSize: 14,
                      color: Color(0xff3a57e8),
                    ),
                  ),
                ),
                Padding(
                  padding: EdgeInsets.fromLTRB(0, 30, 0, 20),
                  child: MaterialButton(
                    onPressed: () {},
                    color: Color(0xff3a57e8),
                    elevation: 0,
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12.0),
                    ),
                    padding: EdgeInsets.all(16),
                    child: Text(
                      "Login",
                      style: TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                    textColor: Color(0xffffffff),
                    height: 50,
                    minWidth: MediaQuery.of(context).size.width,
                  ),
                ),
                MaterialButton(
                  onPressed: () {
                    Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (context) => const RegisterCat(),
                      ),
                    );
                  },
                  color: Color(0x2d3a57e8),
                  elevation: 0,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12.0),
                  ),
                  padding: EdgeInsets.all(16),
                  child: Text(
                    "Register",
                    style: TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.w700,
                      color: Color(0xff3a57e8),
                    ),
                  ),
                  height: 50,
                  minWidth: MediaQuery.of(context).size.width,
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
