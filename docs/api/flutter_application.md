# Flutter Application API

In this file, there is a description for each of the files in the flutter application.

## Components

### main.dart

The `main.dart` file is the **entry point** for the Flutter application. It initializes the app, sets up global state providers, WebSocket connectivity, action handlers, and the custom router configuration.

| Component                | Purpose                                                     |
| ------------------------ | ----------------------------------------------------------- |
| `MultiProvider`          | Supplies reactive state globally using `Provider` and injecting the three states `GameState`, `LobbyState`, and `PlayerState`.           |
| `WebSocketService`       | Establishes real-time communication with the game server.  |
| `ServerActionDispatcher` | All incoming WebSocket messages are routed via a central ServerActionDispatcher to the handlers: `GameActions`, `LobbyActions`, and `PlayerActions`. |
| `setupActionHandlers()`  | Registers all WebSocket event responses                     |
| `createRouter(...)`      | Builds app navigation logic depending on state using a custom router.      |

### player.dart

Defines the player data structure used throughout the game.

#### Fields
| Name       | Type       | Description             |
|------------|------------|-------------------------|
| `name`     | `String`   | Unique player name      |
| `role`     | `String`   | Either `"hider"` or `"seeker"` |
| `lat`      | `double`   | Latest latitude         |
| `lng`      | `double`   | Latest longitude        |

Only has the functions to be converted to (`toJson()`) and from (`fromJosn(...)`) JSON as it is passed as information in some messages to the server.


### ping_state.dart

Enum for tracking the ping lifecycle.

* `idle`: the ping button is not clicked and no ping is active
* `pinging`: the location of the users obtained from the ping is being shown
* `cooldown`: time before another ping can be executed


### websocket_service.dart

This file implements the **WebSocketService**, a central class responsible for managing the app’s WebSocket connection. It enables real-time communication between the client and server, handles connection lifecycle events, and relays incoming messages to the `ServerActionDispatcher`.

#### Connection Methods

| **Component** | **Purpose** |
|---------------|-------------|
| `connect(...)` | Connects to the WebSocket server at the given URL. |
| `disconnect()` | Manually disconnects from the WebSocket server by canceling the reconnection logic, closing the `WebSocket` connection, and triggering the disconnect callback. |
| `isConnected()` | Returns `true` if the WebSocket is currently connected. |

#### Messaging Methods

| **Component** | **Purpose** |
|---------------|-------------|
| `send(...)` | Sends a message to the server where: `action` is a `string` key for the action type (received and identified by the handlers in the server) and `data` is a map containing the payload to send. |

The JSON encoding wraps both into:

```json
{
    "action": ...,
    "data": ...
}
```

#### Callbacks

| **Component** | **Purpose** |
|---------------|-------------|
| `setOnConnect(...)` | Registers a callback to be executed when the WebSocket successfully connects. |
| `setOnDisconnect(...)` | Registers a callback to be executed when the WebSocket disconnects or encounters an error. |

#### Internal Logic

| **Component** | **Purpose** |
|---------------|-------------|
| `_initConnection()` | Initializes a WebSocket connection and sets up listeners for messages, errors, and disconnections. |
| `_onMessageRecieved(...)` | Handles incoming messages from the server by passing them to the `dispatcher`. |
| `_handleDisconnect()` | Handles socket closure (normal or error). Triggers reconnect unless manually disconnected. |
| `_handleError(...)` | Handles socket errors, logs them, and triggers reconnection. |
| `_scheduleReconnect()` | Schedules a reconnect using exponential backoff (1s, 2s, 4s,... up to 64s and back to 1s). |

### action_dispatcher.dart

The `ServerActionDispatcher` is a central utility for routing incoming WebSocket messages to their corresponding handlers based on an `action` string. It decouples message reception from handling logic, allowing for clean and modular message processing.

It stores handlers in a private map keyed by action strings:

``` dart
final Map<String, ServerActionHandler> _handlers = {};
```

| **Component** | **Purpose** |
|---------------|-------------|
| `register(...)` | Registers a message handler for a specific action type. For instance, `get_ping` is registered for the handler `GameActions`. |
| `handleMessage(...)` | Parses and dispatches an incoming JSON-formatted string. If the `action` matches a registered handler, the handler is invoked with the `data`. If no matching handler is found, logs: "Unhandled action: someAction". |

### player_handler.dart

The `PlayerActions` class defines message handlers related to player events and registration. It connects WebSocket events with updates to the `PlayerState` provider in Flutter.

The `register(...)` method registers all the server actions to the handlers.

<table>
  <thead>
    <tr>
      <th><strong>Action</strong></th>
      <th><strong>Purpose</strong></th>
      <th><strong>Payload</strong></th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><code>player_registered</code></td>
      <td>The server confirms that a player has been registered (potentially authenticated).</td>
      <td>
        <pre><code class="language-json">{
  "username": "..."
}</code></pre>
      </td>
    </tr>
  </tbody>
</table>




### lobby_handler.dart

The `LobbyActions` class defines all server-side WebSocket event handlers related to lobby management. It updates app state (`LobbyState`, `PlayerState`, and `GameState`) and manages error dialogs using the Flutter UI context.

The `register(...)` method registers all the server actions to the handlers.

<table>
  <thead>
    <tr>
      <th><strong>Action</strong></th>
      <th><strong>Purpose</strong></th>
      <th><strong>Payload</strong></th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><code>lobby_joined</code></td>
      <td>A player joins an existing lobby. Updates <code>PlayerState</code> with the joined player, Initializes <code>LobbyState</code> with the ID and resets the <code>GameState</code>.</td>
      <td>
        <pre><code class="json">{
  "lobbyId" : "...",
  "player": {
    "name": "...",
    "role": "Seeker",
    "nickname": "Ali"
  }
}</code></pre>
      </td>
    </tr>
    <tr>
      <td><code>lobby_created</code></td>
      <td>A new lobby is successfully created. Updates <code>PlayerState</code> and sets the host flag to <code>true</code> in <code>LobbyState</code> because it is the first player to join that lobby.</td>
      <td><code>Same as <code>lobby_joined</code></code></td>
    </tr>
    <tr>
      <td><code>new_player_joined</code></td>
      <td>A new player joins the same lobby as the user. Adds the new player to the current lobby's player list.</td>
      <td>
        <pre><code class="json">{
  "player": {
    "name": "...",
    "role": "...",
    "nickname": "..."
  }
}</code></pre>
      </td>
    </tr>
    <tr>
      <td><code>leave_lobby</code></td>
      <td>A player leaves or the lobby is deleted. Shows an error and removes the player from the lobby.</td>
      <td>None</td>
    </tr>
    <tr>
      <td><code>new_host</code></td>
      <td>The host leaves and another player is assigned as the new host (next player in the list).</td>
      <td>None</td>
    </tr>
    <tr>
      <td><code>lobby_deleted</code></td>
      <td>The entire lobby is deleted by the host or system. Shows an error and clears the entire lobby state.</td>
      <td>None</td>
    </tr>
    <tr>
      <td><code>player_list</code></td>
      <td>The server sends an update list of the players in the <code>LobbyState</code>.</td>
      <td>
        <pre><code class="json">{
  "players": [ player1_info, player2_info, ... ]
}</code></pre>
      </td>
    </tr>
  </tbody>
</table>

#### Error Handlers

The error handlers catch the following errors:
* `error_creating_lobby`
* `player_already_in_lobby`
* `lobby_does_not_exists`
* `error` (for a general error)

The method `_showError(...)` uses `rootNavigatorKey.currentContext` to dusplay an alert dialog.

### game_handler.dart

The `GameActions` class registers WebSocket event handlers for **game lifecycle** events. These handlers control transitions from lobby to game, update game-specific states, and manage player interactions during gameplay.

The `register(...)` method registers all the server actions to the handlers.

<table>
  <thead>
    <tr>
      <th><strong>Action</strong></th>
      <th><strong>Purpose</strong></th>
      <th><strong>Payload</strong></th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><code>game_started</code></td>
      <td>Message that the game has been started by the host. Updates the local <code>LobbyState</code> with the full list of players.</td>
      <td>
        None
      </td>
    </tr>
    <tr>
      <td><code>location_request</code></td>
      <td>The server requests the player's current location. Retrieves the current player location from <code>GameState</code> and sends the location back to the server.</td>
      <td>None</td>
    </tr>
    <tr>
      <td><code>location_update_list</code></td>
      <td>The server sends a list of updated hider locations (to be shown to seekers). Updates ping UI and updates hiders positions in <code>GameState</code></td>
      <td>
        <pre><code class="json">{
  "players": [ 
    { "name": "...", "lat": ..., "lon": ... },
    { "name": "...", "lat": ..., "lon": ... },
    ...
  ]
  }
</code></pre>
      </td>
    </tr>
    <tr>
      <td><code>time_update</code></td>
      <td>The server sends updated game time and synchronization offset. Updates timers in <code>GameState</code></td>
      <td>
      <pre><code class="json">{
  "time": "...",
  "time_offset": "..."
  }
</code></pre>
</td>
    </tr>
    <tr>
      <td><code>game_ended</code></td>
      <td>The game conditions are fullfiled and the game concludes.</td>
      <td>None</td>
    </tr>
  </tbody>
</table>

### message_sender.dart

The `MessageSender` class provides a centralized interface for sending WebSocket messages to the server, encapsulating communication logic for:

- Player authentication
- Lobby lifecycle events
- In-game updates like location sharing or game start signals

All messages are sent using the globally accessible `webSocketService.send(...)`.

#### Player Methods

<table>
  <thead>
    <tr>
      <th><strong>Action</strong></th>
      <th><strong>Purpose</strong></th>
      <th><strong>Payload</strong></th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><code>login_player</code></td>
      <td>Sends a login request to register or authenticate a player.</td>
      <td>
        <pre><code class="json">{
  "deviceId": "...",
  "username": "...",
  }
</code></pre>
      </td>
    </tr>
  </tbody>
</table>

#### Lobby Methods

<table>
  <thead>
    <tr>
      <th><strong>Action</strong></th>
      <th><strong>Purpose</strong></th>
      <th><strong>Payload</strong></th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><code>create_lobby</code></td>
      <td>Requests server to create a new lobby.</td>
      <td>
        <pre><code class="json">{
  "lobbyId": "...",
  "username": "...",
  "role": "..."
  }
</code></pre>
      </td>
    </tr>
    <tr>
      <td><code>join_lobby</code></td>
      <td>Requests to join in a (in theory) existing lobby.</td>
      <td>
        <pre><code class="json">{
  "lobbyId": "...",
  "username": "...",
  "nickname": "...",
  "role": "..."
  }
</code></pre>
      </td>
    </tr>
    <tr>
      <td><code>exit_lobby</code></td>
      <td>Tells the server that a player wants to leave a lobby.</td>
      <td>
        <pre><code class="json">{
  "lobbyId": "...",
  "username": "..."
  }
</code></pre>
      </td>
    </tr>
    </tr>
    <tr>
      <td><code>delete_lobby</code></td>
      <td>Deletes the current lobby. Only possible by the host.</td>
      <td>
        <pre><code class="json">{
  "lobbyId": "..."
  }
</code></pre>
      </td>
    </tr>
  </tbody>
</table>

#### Game Methods

<table>
  <thead>
    <tr>
      <th><strong>Action</strong></th>
      <th><strong>Purpose</strong></th>
      <th><strong>Payload</strong></th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><code>update_position</code></td>
      <td>Sends real-time GPS coordinates to the server.</td>
      <td>
        <pre><code class="json">{
  "username": "...",
  "lobbyId": "...",
  "lat": ...,
  "lon": ...
  }
</code></pre>
      </td>
    </tr>
    <tr>
      <td><code>start_game</code></td>
      <td>Signals the server that the host wants to start the game.</td>
      <td>
        <pre><code class="json">{
  "lobbyId": "..."
  }
</code></pre>
      </td>
    </tr>
    <tr>
      <td><code>ping_request</code></td>
      <td>A seeker has requested to do the ping of the hider's locations. It is requesting for those.</td>
      <td>
        <pre><code class="json">{
  "username": "...",
  "lobbyId": "..."
  }
</code></pre>
      </td>
    </tr>
  </tbody>
</table>

## User Interface (UI)

### router.dart

In `routes:` the router defines the routes and the pages it needs to redirect to. For example:

```dart
GoRoute(path: '/lobby', builder: (context, state) => const LobbyPage())
```

This route with path `/lobby`, when accessed is going to build the page `LobbyPage`.

In `redirect:` the router defines all the state-based logic to know, based on the `PlayerState`, `LobbyState`, and `GameState` which screen should be shown. The Conditions and pages are the following:

| **Condition** | **Description** | **Redirect To** |
|---------------|-------------|-------------|
| `username == null` | The username is null when the player has not yet been authenticated. | `/auth` |
| `gameEnded == true && playing == true` | The player was playing until now, but the game has already ended. | `/end` |
| `hasUsername && inLobby && playing && !gameEnded` | The player is authenticated, is assigned to a lobby, the player is in a lobby where the game has started, and the game has not been set to end. | `/game` |
| `hasUsername && inLobby && !playing && !gameEnded` | The username is authenticated, is in a lobby, but they are not playing and the game didn't even start. | `/lobby` |
| `hasUsername && !inLobby && !playing` | The user is authenticated, they are not yet in a lobby, and are not playing. | `/home` |

We use ``Provider`` with ``ChangeNotifier`` to manage state across the app, and these three states are deeply connected to the routing. At each change in the state, the function `notifyListeners()` to update the components dependent on the states.

### player_state.dart

| Property      | Purpose                                  |
| ------------- | ---------------------------------------- |
| `username`    | Set when the player has logged in and been authenticated.           |
| `nickname`    | Used inside a lobby                          |
| `isConnected` | Tracks WebSocket connection              |
| `isOnline`    | Used for presence management, and lobby logic             |
| `_player`     | Holds full Player model (with role etc.) |

The methods are pretty straightforward.

### lobby_state.dart

| Property  | Purpose                          |
| --------- | -------------------------------- |
| `lobbyId` | Identifies current lobby         |
| `isHost`  | True if player created the lobby |
| `players` | List of all players in the lobby |
| `playing` | True if game has started         |

The methods are pretty straightforward.

### game_state.dart

| Property         | Purpose                                  |
| ---------------- | ---------------------------------------- |
| `isHider`        | Player’s role (affects ping logic)       |
| `pingState`      | Tracks if pinging, cooling down, or idle |
| `userLocation`   | Tracks device location                   |
| `hiders/seekers` | Player roles split                       |
| `gameEnded`      | Signals game end (navigates to `/end`)   |
| `remainingTime`  | Countdown timer (signals end of game when it reaches 0)                         |


### Pages & Widgets

Some of the pages and widgets use a `Consumer`. It is a widget provided by the provider package that listens to a specific ChangeNotifier and rebuilds only the widget(s) below it when that notifier changes.

The widget tree in the pages are the following:

```text
MaterialApp.router
└── GoRouter (router config)
    ├── /auth → AuthPage
    │   └── Scaffold
    │       ├── AppBar
    │       │   └── Text("Player Login")
    │       └── Padding (24.0)
    │           └── Column (centered)
    │               ├── TextField
    │               │   ├── controller: _usernameController
    │               │   └── decoration: InputDecoration
    │               │       ├── labelText: "Username"
    │               │       └── border: OutlineInputBorder
    │               ├── SizedBox (height: 20)
    │               └── ElevatedButton
    │                   ├── onPressed: _login
    │                   └── child: Text("Log in")
    │
    ├── /home → HomePage
    │   └── Scaffold
    │       ├── AppBar
    │       │   └── Text("Multiplayer Lobby - {username}")
    │       └── Center
    │           └── Padding (horizontal: 24, vertical: 48)
    │               └── Column (centered)
    │                   ├── ElevatedButton.icon ("Join Lobby")
    │                   │   ├── Icon(Icons.login)
    │                   │   ├── Text("Join Lobby")
    │                   │   └── onPressed: → CreateLobbyPage
    │                   │                       └── Scaffold
    │                   │                           ├── AppBar(title: "Create Lobby")
    │                   │                           └── Body: Padding (24 all around)
    │                   │                               └── Column
    │                   │                                   ├── TextField (controller: _lobbyNameController, label: "Lobby Name")
    │                   │                                   ├── SizedBox(height: 20)
    │                   │                                   ├── DropdownButton<String> (value: _selectedRole)
    │                   │                                   │   ├── Items: ["Hider", "Seeker"]
    │                   │                                   ├── SizedBox(height: 20)
    │                   │                                   └── ElevatedButton ("Create", onPressed: _createLobby)
    │                   ├── SizedBox(height: 20)
    │                   └── ElevatedButton.icon ("Create Lobby")
    │                       ├── Icon(Icons.add_box)
    │                       ├── Text("Create Lobby")
    │                       └── onPressed: → JoinLobbyPage
    │                                           └── Scaffold
    │                                               ├── AppBar (title: "Join Lobby")
    │                                               └── Padding (all: 24)
    │                                                   └── Column
    │                                                       ├── TextField (Lobby Name)
    │                                                       ├── SizedBox(height: 20)
    │                                                       ├── TextField (Nickname)
    │                                                       ├── SizedBox(height: 20)
    │                                                       ├── DropdownButton (Hider / Seeker)
    │                                                       ├── SizedBox(height: 20)
    │                                                       └── ElevatedButton ("Join Lobby")
    │
    ├── /lobby → LobbyPage
    │   └── Scaffold
    │       ├── AppBar
    │       │   ├── Text("Lobby: {lobbyId}")
    │       │   └── (if isHost) Icon(Icons.star)
    │       ├── Body: Column
    │       │   ├── Expanded
    │       │   │   └── ListView.builder
    │       │   │       └── PlayerListTile (for each player)
    │       │   └── Padding (16)
    │       │       └── Column
    │       │           ├── FractionallySizedBox (Leave Lobby button)
    │       │           │   └── ElevatedButton.icon
    │       │           ├── (if isHost)
    │       │           │   ├── SizedBox(height: 12)
    │       │           │   └── FractionallySizedBox (Delete Lobby button)
    │       │           │       └── ElevatedButton.icon
    │       └── (if isHost)
    │           └── FloatingActionButton (Start Game)
    │
    ├── /game → MapPage
    │   └── Scaffold
    │       ├── AppBar (Title: "Map")
    │       └── Body: Stack
    │           ├── Positioned.fill
    │           │   └── HiderMapView or SeekerMapView (based on role)
    │           ├── Positioned (top-left)
    │           │   └── Consumer<GameState>
    │           │       └── Countdown Timer Container
    │           └── Positioned (bottom-right)
    │               └── ElevatedButton.icon ("Leave Game")
    │                   └── AlertDialog (on tap)
    │
    └── /end → EndGamePage
        └── Scaffold
            ├── AppBar (title: "Game Over")
            └── Body: Center
                └── Column (centered vertically)
                    ├── Text (resultMessage, bold, size 24)
                    ├── (optional) SizedBox(height: 12)
                    ├── (optional) Text (additionalInfo)
                    ├── SizedBox(height: 24)
                    └── ElevatedButton ("Return to Home")
```