# Welcome to the respository for Cat&Seek!

In this repository we have the source code and documentation for a real-time mobile game built using **Flutter (Dart)** for the frontend and **C# (.NET)** for the backend server.

This project brings the classic childhood game of *Hide & Seek* into the digital world, allowing players to join multiplayer sessions, take on the role of a hider or a seeker, and play in real time with others over a network connection.

# 📚 Table of Contents

- 🎮 [Gameplay Overview](#-gameplay-overview)
- 📁 [Repository Structure](#-repository-structure)
- 🛠️ [Tech Stack](#️-tech-stack)
- 📄 [Documentation](#documentation)

# 🎮 Gameplay Overview

The game allows multiple players to:
- 📲 Connect via mobile devices using a Flutter app
- 🌐 Join or host game lobbies
- 👤 Be assigned as a **hider** or **seeker**
- 🗺️ Navigate a map and look for players via *pings*
- 🧩 Complete **interactive teamworking tasks** to unlock special **abilities** that give players an edge during the game (e.g., extra pings, make phone sounds, etc.).
- 🎯 **Eliminate opponents by scanning a QR codes** (e.g., Seekers scan Hiders' QR codes to tag them out).

# 📁 Repository Structure

The structure of the repository is the following, only showing the relevant files and folders for the development and understanding of the project:

```bash
/
├── semester_project/     # Flutter mobile app
│   ├── pubspec.yaml          
│   ├── lib/                  # code files
│   │   ├── main.dart
│   │   └── ...
│   ├── assets/               
├── server/               # C# server project
│   ├── WebApplication1/      # .NET program
│   │   ├── Program.cs
│   │   └── ...
├── docs/                 # Extended documentation
│   ├── architecture.md
│   ├── api/
│   │   ├── flutter_application.md
│   │   └── server_program.md
│   ├── setup/
│   │   ├── flutter-setup.md
│   │   └── server-setup.md
├── assets/				  # assets for the documentation
├── FUTUREWORK.md 
└── README.md
```

# 🛠️ Tech Stack

## 📱 **Frontend (Mobile App)**

- **Framework**: [Flutter](https://flutter.dev/) (Dart)
- **Device Identifiaction**: `uuid`
- **State Management**: `provider` (to share data across the app)
- **Routing & Navigation**: 
	- `go_router` (routing package to handle navigation between screens)
	- `framework` (Flutter default)
- **QR Scanning**: `qr_flutter` (for generating QR codes)
- **Geolocation & Map Displaying**:
	- `flutter_map` (to display interactive maps)
	- `geolocator` (to access the device's real-time GPS location)
	- `latlong2` (data class for geographic coordinates, used with `flutter_map` and `geolocator`)
- **Networking**:
    - `dart:io` (for creating and managing the WebSocket connections via `WebSocket.connect`)
    - `dart_async` (to handle connection retries with `Timer`)
- **Messaging**:
	- `dart:convert` (for encoding and decoding JSON data with `jsonEncode` and `jsonDecode`)

For the non-native dart libraries, they are all included in the `pubspect.yaml` file in the frontend `semester_project` folder:

```yaml
dependencies:
  flutter:
    sdk: flutter 

  # Device identification
  uuid: ^4.4.2  

  # State Management
  provider: ^6.1.1
  
 # and others ...
```

## 🖥️ **Backend (Server)**

- **Language**: C#
- **Framework**: ASP.NET Core (Web API)
- **Database**: `System.Data.SQLite`
- **Real-time Communication**:
    - WebSockets (`System.Net.WebSockets`)
    - Concurrent Environaments with thread-safe collection classes (`System.Collections.Concurrent`)
- **Messaging**: 
	- General text manipulation (`System.Text`)
	- JSON serialization and deserialization (`System.Text-Json`)

# 📄Documentation

The documentation is all written in the [docs](docs) folder of the repository.

The architecture of the project is explained in the [architecture](docs/architecture.md) file.

The [api](docs/api) folder contains a description of the key concepts and how they are implemented in the code for both:
- [Flutter Application](docs/api/flutter_application.md)
- [Server Program](docs/api/server_program.md)

## Get Started

The guides to setup the computer to run both the Flutter application and the server are in the files:
- [flutter-setup](docs/setup/flutter-setup.md)
- [server-setup](docs/setup/server-setup.md)


# Future Work

The game in this repository is far away from considered finished. There is still a lot to improve and implement. See more details on what can be the next line of work in [FUTUREWORK](FUTUREWORK).

## 📬Contact

For any questions, you can contact us in any of the following emails:
- danielmalheiro10@hotmail.com (backend: general game logic)
- aurorapujolsrial@gmail.com (backend: general game logic, frontend: navigation)
- rodrigocastro27@outlook.pt (backend: web sockets, reconnection protocol)
- dogman039@gmail.com (frontend: general UI)

---

Thank you for checking out this project, and hopefully, continuing it! 🚀