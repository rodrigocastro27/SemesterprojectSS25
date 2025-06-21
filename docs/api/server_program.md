# Server Program API

In this file, there is a description for each of the files in the server program.

### Program.cs

Main entry point for the C# WebSocket server. This file initializes the server, sets up WebSocket routing, handles client connections, and manages real-time communication between clients (e.g., players) and the game lobby.

| Feature            | Description                                                             |
| ------------------ | ----------------------------------------------------------------------- |
| Web Framework      | ASP.NET Core Minimal API                                                |
| Protocol           | WebSocket over `/ws` route. Launches the server on all network interfaces at port 5000.                                              |
| Core Services Used | `SQLiteConnector`, `WebSocketActionDispatcher`, `LobbyManager`, `PlayerManager` |
| Key Handlers       | `LobbyHandlers`, `PlayerHandlers`, `GameHandlers`                       |

🧹 **Error Handling & Cleanup:** It handles disconnection or server errors.

*   Finds the `Player` by their socket.
*   Sets them offline in-memory and in the database.
*   Cleans up their socket reference.
*   Optionally cleans up empty lobbies.

**Flow Overview**

1. Accepts incoming WebSocket requests. 
2. Reads text messages from connected clients. 
3. Decodes message and forwards it to the `WebSocketActionDispatcher`.
4. Catches and handles exceptions for:
   1. `WebSocketException`: network/disconnection issues.
   2. General Exception: unexpected server-side errors.
5. Cleans up user session and updates online status in the database.

## Database

### SQLiteConnector.cs

Provides a centralized, static interface for managing SQLite database connections within the application. This utility class is responsible for initializing the database file, generating a connection string, and offering open connections for querying.

- Dynamically builds the path to the SQLite `.db` file.
- Ensures the database file and its directory exist on first run.
- Provides a method to retrieve an open `SQLiteConnection`.

### DataLoader.cs

Initializes server-side in-memory data structures by loading persistent data from the SQLite database at application startup. It reads from three key tables: `Lobbies`, `Players`, and `LobbyPlayers`.

- Load all lobbies and register them in the `LobbyManager`.
- Load all players and register them in the `PlayerManager`.
- Reconnect online players to lobbies based on saved associations in `LobbyPlayers`.

### DatabaseHandler.cs

Provides an abstraction layer over direct SQLite operations for managing data in `Lobbies`, `Players`, and `LobbyPlayers` tables.

- Perform `INSERT`, `SELECT`, `UPDATE`, and `DELETE` operations on the game database.
- Maintain lobby-player relationships and update online player state.
- Automatically clean up database entries for disconnected players.

## Server Logic