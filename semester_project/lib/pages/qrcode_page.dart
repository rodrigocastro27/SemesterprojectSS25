import 'package:flutter/material.dart';
import 'package:qr_flutter/qr_flutter.dart';

class MyQr extends StatefulWidget
{
  const MyQr({super.key});
   @override
  State<MyQr> createState()=> _MyQrState();
}


class _MyQrState extends State<MyQr>{
    
  String ? data;//needs to be outside build 
  
  @override 
  Widget build(BuildContext context)
  {

    return Scaffold(
        resizeToAvoidBottomInset : false,
        appBar: AppBar(title: Text("QR CODE!"),
        backgroundColor: Colors.deepPurpleAccent,),
        body: Padding(
          padding: const EdgeInsets.all(15.0),
          child: Column(
            children: [
              QrImageView(data: data ?? ''),
              const SizedBox(height: 40,),

              TextField(
                onChanged: (value) {
                  setState(() {
                    data = value;
                  });
                },
                decoration: const InputDecoration(
                  labelText: 'Enter Data'
                ),
              )

            ],
          ),
        )

    );
  }
}