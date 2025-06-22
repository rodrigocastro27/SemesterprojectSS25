# Server Program API

## **Program.cs**

This file serves as the entry point for the C# server application. It configures the web server, initializes the database, sets up WebSocket endpoints, and registers various request handlers for WebSocket communication.

### Server Initialization

| Function                                      | Description                                                                      | Parameters                                        | Return Value |
| --------------------------------------------- | -------------------------------------------------------------------------------- | ------------------------------------------------- | ------------ |
| `SQLiteConnector.Initialize(contentRootPath)` | Initializes the SQLite database connection and ensures the DB file exists.       | `contentRootPath` – Root directory of the project | `void`       |
| `DataLoader.LoadAll()`                        | Loads initial data from the database into memory, including players and lobbies. | None                                              | `void`       |

### Dispatcher Registration

| Function                              | Description                                         | Parameters                                                   | Return Value |
| ------------------------------------- | --------------------------------------------------- | ------------------------------------------------------------ | ------------ |
| `LobbyHandlers.Register(dispatcher)`  | Registers WebSocket handlers related to lobbies.    | `dispatcher` – The shared WebSocketActionDispatcher instance | `void`       |
| `PlayerHandlers.Register(dispatcher)` | Registers WebSocket handlers related to players.    | `dispatcher` – The shared WebSocketActionDispatcher instance | `void`       |
| `GameHandlers.Register(dispatcher)`   | Registers WebSocket handlers related to game logic. | `dispatcher` – The shared WebSocketActionDispatcher instance | `void`       |

### WebSocket Endpoint

| Endpoint | Description                                                                | Parameters                       | Return Value |
| -------- | -------------------------------------------------------------------------- | -------------------------------- | ------------ |
| `/ws`    | Accepts incoming WebSocket requests and routes messages to the dispatcher. | WebSocket connection from client | `void`       |

#### WebSocket Communication Loop

| Function                                | Description                              | Parameters                                                              | Return Value   |
| --------------------------------------- | ---------------------------------------- | ----------------------------------------------------------------------- | -------------- |
| `dispatcher.HandleMessage(msg, socket)` | Processes an incoming WebSocket message. | `msg` – The text message from client<br>`socket` – The WebSocket object | `Task` (async) |

#### Exception Handling and Cleanup

| Function                                                             | Description                                         | Parameters                                                | Return Value |
| -------------------------------------------------------------------- | --------------------------------------------------- | --------------------------------------------------------- | ------------ |
| `PlayerManager.Instance.FindPlayerWithSocket(socket)`                | Retrieves a player by their WebSocket instance.     | `socket` – The client WebSocket                           | `Player?`    |
| `DatabaseHandler.Instance.UpdatePlayersIsOnline(username, isOnline)` | Updates the player's online status in the database. | `username` – Player's name<br>`isOnline` – Boolean status | `void`       |
| `LobbyManager.Instance.DeleteEmptyLobbies()`                         | Removes any lobbies with no active players.         | None                                                      | `void`       |

### Server Execution

| Function                              | Description                                                             | Parameters           | Return Value |
| ------------------------------------- | ----------------------------------------------------------------------- | -------------------- | ------------ |
| `app.RunAsync("http://0.0.0.0:5000")` | Starts the server and begins listening for HTTP and WebSocket requests. | The endpoint address | `Task`       |

## Database

### **SQLiteConnector.cs**

This static class provides access to a SQLite database by managing the database path, connection string, and database initialization.

**Purpose:** establishes and manages connections to a SQLite database and ensures the database file exists.

#### Public Static Methods

| Function                             | Description                                                                                                               | Parameters                                                                                 | Return Value       |
| ------------------------------------ | ------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------ | ------------------ |
| `Initialize(string contentRootPath)` | Initializes the database by constructing the path, connection string, and creating the database file if it doesn't exist. | `contentRootPath` – The root directory of the project (used to determine DB file location) | `void`             |
| `GetConnection()`                    | Returns an open connection to the SQLite database using the internal connection string.                                   | None                                                                                       | `SQLiteConnection` |

#### Private Static Methods

| Function                 | Description                                                                           | Parameters | Return Value |
| ------------------------ | ------------------------------------------------------------------------------------- | ---------- | ------------ |
| `EnsureDatabaseExists()` | Checks if the database file exists; if not, it creates the directory and SQLite file. | None       | `void`       |

#### Internal Fields

| Field               | Type     | Description                                         |
| ------------------- | -------- | --------------------------------------------------- |
| `_dbPath`           | `string` | Full file system path to the SQLite database file   |
| `_connectionString` | `string` | Connection string used to open database connections |

---

### **DatabaseHandler.cs**

This class manages database operations related to players, lobbies, and their relationships. It provides insert, select, update, and delete methods for corresponding SQLite tables.

**Purpose:** acts as a singleton to execute SQL operations using the `SQLiteConnector`. Handles the data layer logic for `Players`, `Lobbies`, and `LobbyPlayers` tables.

#### Singleton Access

```csharp
DatabaseHandler.Instance
```

#### Public Methods

##### INSERT

| Function                                                                                             | Description                                  | Parameters                                          | Return Value |
| ---------------------------------------------------------------------------------------------------- | -------------------------------------------- | --------------------------------------------------- | ------------ |
| `InsertIntoLobbies(string lobbyId)`                                                                  | Inserts a new lobby by name.                 | `lobbyId` – Lobby identifier                        | `void`       |
| `InsertIntoPlayers(string deviceId, string username)`                                                | Adds a player with an online status.         | `deviceId`, `username`                              | `void`       |
| `InsertIntoLobbyPlayers(string lobbyId, string username, string nickname, bool isHost, string role)` | Registers a player to a lobby with metadata. | `lobbyId`, `username`, `nickname`, `isHost`, `role` | `void`       |

##### SELECT

| Function                                        | Description                                    | Parameters | Return Value |
| ----------------------------------------------- | ---------------------------------------------- | ---------- | ------------ |
| `SelectLobbyFromLobbyPlayers(string username)`  | Gets the lobby ID for a player.                | `username` | `string`     |
| `SelectIsHostFromLobbyPlayers(string username)` | Checks if the player is a host in their lobby. | `username` | `bool`       |

##### UPDATE

| Function                                                                    | Description                                                 | Parameters                     | Return Value |
| --------------------------------------------------------------------------- | ----------------------------------------------------------- | ------------------------------ | ------------ |
| `UpdateLobbyPlayersNickname(string username, string nickname, string role)` | Updates a player's nickname and role.                       | `username`, `nickname`, `role` | `void`       |
| `UpdatePlayersIsOnline(string username, bool isOnline)`                     | Sets player's online status; deletes from lobby if offline. | `username`, `isOnline`         | `void`       |
| `UpdateLobbyPlayersLobby(string username, string lobbyId, string role)`     | Changes the player’s lobby and role.                        | `username`, `lobbyId`, `role`  | `void`       |
| `UpdateLobbyPlayersHost(string username)`                                   | Promotes a player to lobby host.                            | `username`                     | `void`       |

##### DELETE

| Function                                                             | Description                             | Parameters            | Return Value |
| -------------------------------------------------------------------- | --------------------------------------- | --------------------- | ------------ |
| `DeleteFromLobbies(string lobbyId)`                                  | Removes a lobby and all its players.    | `lobbyId`             | `void`       |
| `DeleteFromPlayers(string username)`                                 | Deletes a player from the system.       | `username`            | `void`       |
| `DeleteFromLobbyPlayersLobbyPlayer(string lobbyId, string username)` | Removes a specific player from a lobby. | `lobbyId`, `username` | `void`       |
| `DeleteFromLobbyPlayersLobby(string lobbyId)`                        | Removes all players from a lobby.       | `lobbyId`             | `void`       |
| `DeleteFromLobbyPlayersPlayer(string username)`                      | Removes a player from any lobby.        | `username`            | `void`       |

---

### **DataLoader.cs**

The `DataLoader` class is responsible for bootstrapping the application by loading persisted data from the SQLite database into memory. This includes lobbies, players, and lobby-player mappings.

**Purpose:** loads the application state into memory by reading from the SQLite database and initializing runtime managers (`LobbyManager`, `PlayerManager`).

#### Public Methods

| Function    | Description                                                                       | Parameters | Return Value |
| ----------- | --------------------------------------------------------------------------------- | ---------- | ------------ |
| `LoadAll()` | Loads all required runtime data: lobbies, players, and player-lobby associations. | None       | `void`       |

#### Private Methods

| Function             | Description                                                                                                                                           | Parameters | Return Value |
| -------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------- | ---------- | ------------ |
| `LoadLobbies()`      | Reads all lobbies from the `Lobbies` table and registers them with `LobbyManager`.                                                                    | None       | `void`       |
| `LoadPlayers()`      | Reads all players from the `Players` table and registers them with `PlayerManager`.                                                                   | None       | `void`       |
| `LoadLobbyPlayers()` | Reads all entries from `LobbyPlayers`, maps players to lobbies, and updates their roles and nicknames. Also cleans up stale data for offline players. | None       | `void`       |

---


## Program

### **WebSocketActionDispatcher.cs**

This utility class enables dynamic dispatching of WebSocket messages based on an `"action"` field in incoming JSON messages. It acts as a message router for WebSocket-based communication.

**Purpose:** routes incoming WebSocket messages to corresponding handler functions based on the `"action"` field in the message.


#### Public Methods

| Function                                                              | Description                                                                           | Parameters                                                                                                             | Return Value   |
| --------------------------------------------------------------------- | ------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------- | -------------- |
| `Register(string action, Func<JsonElement, WebSocket, Task> handler)` | Registers a handler function for a specific action keyword.                           | `action` – String name of the action<br>`handler` – Function that handles the action, takes JSON payload and WebSocket | `void`         |
| `HandleMessage(string jsonMessage, WebSocket socket)`                 | Parses an incoming message and dispatches it to the appropriate handler if available. | `jsonMessage` – The raw JSON string from the client<br>`socket` – The active WebSocket connection                      | `Task` (async) |

#### Behavior Details

* Expected message structure:

```json
{
  "action": "someAction",
  "data": { /* arbitrary payload */ }
}
```

* If either `"action"` or `"data"` is missing, the message is logged as malformed.
* If the `"action"` value is not found in the registered handlers, it's logged as unknown.

---

### **MessageSender.cs**

This static utility class provides methods to send JSON-formatted WebSocket messages to individual players or broadcast to lobbies, filtered by role if needed.

**Purpose:** facilitates structured message communication over WebSockets between the server and connected players/lobbies in the game.

#### Public Static Methods

| Function                                                       | Description                                                                | Parameters                                                                                                      | Return Value |
| -------------------------------------------------------------- | -------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------- | ------------ |
| `SendAsync(WebSocket socket, string action, object data)`      | Sends a JSON message with an action and payload to a specific WebSocket.   | `socket` – Target WebSocket<br>`action` – Action string<br>`data` – Payload object                              | `Task`       |
| `SendToPlayerAsync(Player player, string action, object data)` | Sends a message to a single player, if their socket is open.               | `player` – Target player<br>`action` – Action string<br>`data` – Payload object                                 | `Task`       |
| `BroadcastLobbyAsync(Lobby lobby, string action, object data)` | Sends a message to all players in the lobby with active sockets.           | `lobby` – Lobby whose players should receive the message<br>`action` – Action string<br>`data` – Payload object | `Task`       |
| `BroadcastToHiders(Lobby lobby, string action, object data)`   | Sends a message only to players with the `hider` role in the given lobby.  | `lobby` – Lobby to filter players<br>`action` – Action string<br>`data` – Payload object                        | `Task`       |
| `BroadcastToSeekers(Lobby lobby, string action, object data)`  | Sends a message only to players with the `seeker` role in the given lobby. | `lobby` – Lobby to filter players<br>`action` – Action string<br>`data` – Payload object                        | `Task`       |

#### Message Format

All messages sent follow this JSON structure:

```json
{
  "action": "yourActionName",
  "data": { /* any serializable content */ }
}
```

---

### **Player.cs**

The `Player` class represents a player in the game. It holds identifying info, WebSocket connection, lobby-related data, role, position, and online state.

**Purpose:** encapsulates all data and behavior related to a player, including role, location, nickname, and connection state.

#### Properties

| Property                | Type          | Description                                            |
| ----------------------- | ------------- | ------------------------------------------------------ |
| `Name`                  | `string`      | The username of the player.                            |
| `Nickname`              | `string`      | Custom name used in the lobby.                         |
| `Id`                    | `string`      | Device ID associated with the player.                  |
| `Socket`                | `WebSocket`   | WebSocket connection to the client.                    |
| `IsHost`                | `bool`        | Whether the player is the lobby host.                  |
| `Position`              | `GeoPosition` | The last known geolocation of the player.              |
| `RoleEnum` *(private)*  | `Role`        | Internal role enum.                                    |
| `Role` *(public field)* | `string`      | Defaulted to `"hider"`, may be used for external sync. |
| `LastLocationUpdate`    | `DateTime`    | Timestamp of the last position update.                 |
| `isOnline` *(private)*  | `bool`        | Whether the player is currently online.                |

#### Methods

| Function                  | Description                                                      | Parameters        | Return Value |
| ------------------------- | ---------------------------------------------------------------- | ----------------- | ------------ |
| `UpdateLocation(pos)`     | Updates the player's geolocation and timestamp.                  | `GeoPosition pos` | `void`       |
| `SetHost(state)`          | Sets the host flag.                                              | `bool state`      | `void`       |
| `SetNickname(nickname)`   | Sets the player's nickname.                                      | `string nickname` | `void`       |
| `SetRole(role)`           | Sets the player's role using enum.                               | `Role role`       | `void`       |
| `SetRole(role)`           | Sets the player's role from string.                              | `string role`     | `void`       |
| `GetRole_s()`             | Returns role as string.                                          | None              | `string`     |
| `GetRole()`               | Returns role as enum.                                            | None              | `Role`       |
| `IsLocationFresh(maxAge)` | Checks if the last location update is within a certain timespan. | `TimeSpan maxAge` | `bool`       |
| `SetOnline(isOnline)`     | Sets online status and removes player from lobby.                | `bool isOnline`   | `void`       |
| `IsOnline()`              | Returns whether the player is online.                            | None              | `bool`       |

#### Struct: `GeoPosition`

| Property    | Type     | Description              |
| ----------- | -------- | ------------------------ |
| `Latitude`  | `double` | Latitude of the player.  |
| `Longitude` | `double` | Longitude of the player. |

Represents a 2D geographic position for location tracking.

#### Enum: `Role`

Defines roles a player can have in the game.

| Value    | Description                     |
| -------- | ------------------------------- |
| `hider`  | A player hiding from seekers.   |
| `seeker` | A player trying to find hiders. |


---

### **Lobby.cs**

The `Lobby` class represents a game lobby that contains a list of players and optionally a game session.

**Purpose:** manages a group of connected players, their roles, and associated game session.

#### Properties

| Property  | Type           | Description                                 |
| --------- | -------------- | ------------------------------------------- |
| `Id`      | `string`       | The unique identifier of the lobby.         |
| `Players` | `List<Player>` | The list of players currently in the lobby. |

#### Methods

| Method                    | Description                                            | Parameters            | Return Value          |
| ------------------------- | ------------------------------------------------------ | --------------------- | --------------------- |
| `AddPlayer(player)`       | Adds a player to the lobby.                            | `Player player`       | `void`                |
| `RemovePlayer(player)`    | Removes a player from the lobby.                       | `Player player`       | `void`                |
| `HasPlayer(player)`       | Checks if the player is in the lobby.                  | `Player player`       | `bool`                |
| `GetRandomPlayer()`       | Returns a random player (currently returns the first). | None                  | `Player`              |
| `SetGameSession(current)` | Sets the current game session.                         | `GameSession current` | `void`                |
| `GetGameSession()`        | Retrieves the current game session.                    | None                  | `GameSession?`        |
| `ClearGameSession()`      | Placeholder for session cleanup logic.                 | None                  | `void`                |
| `GetHidersList()`         | Returns a list of players with role `hider`.           | None                  | `List<Player>`        |
| `GetSeekerList()`         | Returns a list of players with role `seeker`.          | None                  | `List<Player>`        |
| `GetSeekersList()`        | Alias of `GetSeekerList()`, returns all seekers.       | None                  | `IEnumerable<Player>` |

#### Notes

* `GetRandomPlayer()` currently returns the first player instead of selecting randomly. Consider using `Random` for actual random selection.
* `ClearGameSession()` is defined but currently empty; implement cleanup logic if needed.

---

### **GameSession.cs**

The `GameSession` class manages the lifecycle and logic of a game session, including timers, task execution, player interactions, and in-game events like pings and eliminations.

**Purpose:** manages game-related logic for a Lobby, including timers, player visibility (ping), tasks, and player elimination.

#### Fields

| Field                  | Type                                              | Description                                 |
| ---------------------- | ------------------------------------------------- | ------------------------------------------- |
| `_lobby`               | `Lobby`                                           | The lobby this session is tied to.          |
| `_playerGameSessions`  | `ConcurrentDictionary<Player, PlayerGameSession>` | Stores per-player session data.             |
| `_taskList`            | `List<GameTask>`                                  | Registered tasks that can be spawned.       |
| `_random`              | `Random`                                          | Random number generator for task selection. |
| `_centerMap`           | `Tuple<double, double>?`                          | Optional center point of the game map.      |
| `_timer`               | `TimeSpan`                                        | Remaining game time.                        |
| `_tickRate`            | `TimeSpan`                                        | Game loop update interval (1s).             |
| `_taskInterval`        | `TimeSpan`                                        | Task spawn interval (15s).                  |
| `_lastTaskSpawnTime`   | `DateTime`                                        | Timestamp of last task spawn.               |
| `_pingCompletedSource` | `TaskCompletionSource<bool>?`                     | Awaitable ping response tracker.            |

#### Lifecycle Methods

| Method      | Description                                                                  |
| ----------- | ---------------------------------------------------------------------------- |
| `Start()`   | Begins the game session, initializes players, and starts update/ping timers. |
| `EndGame()` | Ends the game, notifies players, and clears the game session.                |

#### Timer Adjustment

| Method                   | Parameters          | Description                        |
| ------------------------ | ------------------- | ---------------------------------- |
| `AddTime(duration)`      | `TimeSpan duration` | Adds time to the session.          |
| `SubtractTime(duration)` | `TimeSpan duration` | Subtracts time and clamps to zero. |

#### Ping Mechanic

| Method                 | Description                                                   |
| ---------------------- | ------------------------------------------------------------- |
| `RequestPing(player)`  | Triggers a location ping.                                     |
| `HandlePing()`         | Returns a `Task` that completes when the ping cycle finishes. |
| `InternalHandlePing()` | Sends location requests and filters fresh hider locations.    |

#### Periodic Update & Tasks

| Method                                               | Description                                                 |
| ---------------------------------------------------- | ----------------------------------------------------------- |
| `UpdateLoop()`                                       | Main loop checking if new tasks should spawn.               |
| `RegisterTask(task)`                                 | Adds a `GameTask` to the available pool.                    |
| `SpawnRandomTask()`                                  | Selects a random task from registered tasks.                |
| `StartTask(lobby)`                                   | Broadcasts a task to all players and waits for interaction. |
| `WaitForAllPlayersUpdateAsync(lobby, task, timeout)` | Waits for all players to respond to a task or times out.    |
| `GetTask(name)`                                      | Retrieves a task by name.                                   |

#### Accessors

| Method                         | Return Type          | Description                          |
| ------------------------------ | -------------------- | ------------------------------------ |
| `GetPlayerGameSession(player)` | `PlayerGameSession?` | Retrieves session data for a player. |
| `GetLobby()`                   | `Lobby`              | Returns the associated lobby.        |

#### Elimination

| Method                    | Description                                     |
| ------------------------- | ----------------------------------------------- |
| `EliminatePlayer(player)` | Marks a player as eliminated and broadcasts it. |

---

### **PlayerGameSession.cs**

Manages the state and actions of an individual player within a specific game session.

**Purpose:** Manages the state, abilities, and task interactions of a single player within an ongoing game session, including visibility and elimination status.


#### Fields (Private)

| Field          | Type             | Description                                          |
| -------------- | ---------------- | ---------------------------------------------------- |
| `_player`      | `Player`         | The associated player object.                        |
| `_gameSession` | `GameSession`    | The current game session.                            |
| `_isVisible`   | `bool`           | Whether the player is currently visible in the game. |
| `_abilities`   | `List<IAbility>` | Player's granted abilities.                          |

#### Properties

| Property       | Type                              | Access      | Description                                           |
| -------------- | --------------------------------- | ----------- | ----------------------------------------------------- |
| `TaskUpdates`  | `Dictionary<string, JsonElement>` | Readonly    | Stores task update info keyed by task name.           |
| `IsEliminated` | `bool`                            | Private set | Whether the player has been eliminated from the game. |

#### Public Methods

| Method                                                       | Description                                                                    |
| ------------------------------------------------------------ | ------------------------------------------------------------------------------ |
| `void GrantAbility(IAbility ability)`                        | Adds a new ability to the player’s ability list.                               |
| `bool HasAbility(IAbility ability)`                          | Checks if player has a specific ability.                                       |
| `IAbility GetAbilityFromString(string abilityName)`          | Finds an ability by its name string; returns `null` if not found.              |
| `Task UseAbility(IAbility ability)`                          | Uses an ability asynchronously, removing it afterward and handling exceptions. |
| `List<IAbility> GetAbilityList()`                            | Returns the current list of abilities the player holds.                        |
| `void MarkUpdateReceived(string taskName, JsonElement info)` | Records an update from the player for a given task.                            |
| `bool HasSentUpdate(string taskName)`                        | Checks if the player has sent an update for a given task.                      |
| `void Eliminate()`                                           | Marks player as eliminated and sends a notification message.                   |
| `JsonElement GetInfoFrom(string taskName)`                   | Retrieves the stored task info for the given task name.                        |
| `Player? GetPlayer()`                                        | Returns the underlying `Player` object.                                        |
| `void SetVisible(bool visible)`                              | Sets the player's visibility state.                                            |
| `bool CheckVisibility()`                                     | Returns `true` if player is visible and not eliminated; otherwise `false`.     |

#### Behavior Notes

* **Abilities:** Abilities are one-time use; they are removed and disposed after being used.
* **Visibility:** Visibility affects whether the player is considered "visible" in game logic, and eliminated players are always invisible.
* **TaskUpdates:** Used to track player responses for game tasks.
* **Elimination:** Marks player eliminated and notifies all relevant parties via messaging service.

---

### **GameTask.cs**

Defines the abstract base class that structures game tasks, enforcing lifecycle methods for execution, completion, and periodic updates.

**Purpose:** Defines the blueprint for game tasks, including their execution, completion handling, and optional periodic updates within a lobby context.

#### Properties

| Property      | Type   | Access   | Description                      |
| ------------- | ------ | -------- | -------------------------------- |
| `Name`        | string | Readonly | The unique name of the task.     |
| `Description` | string | Readonly | A brief explanation of the task. |

#### Abstract Methods

| Method                                                                    | Description                                                         |
| ------------------------------------------------------------------------- | ------------------------------------------------------------------- |
| `Task ExecuteAsync(Lobby lobby)`                                          | Defines what happens when the task starts, executed asynchronously. |
| `Task EndTask(Lobby lobby, HashSet<PlayerGameSession> respondedSessions)` | Handles task completion logic, typically after player updates.      |

#### Virtual Methods

| Method                          | Description                                            |
| ------------------------------- | ------------------------------------------------------ |
| `Task OnTickAsync(Lobby lobby)` | Optional periodic update method during task execution. |

#### Usage Notes

* Extend this class to create concrete game tasks.
* Each task must implement the core lifecycle methods to be functional.
* Designed for asynchronous operation to fit real-time multiplayer scenarios.

---


### **ClickingRaceTask.cs**

Implements a competitive timed clicking task where hiders and seekers race to click a button as many times as possible, awarding abilities to the top performers.

**Purpose:** Implements a competitive timed clicking task where hiders and seekers race to click a button as many times as possible, awarding abilities to the top performers.

#### Private Fields

| Field             | Type | Description                                 |
| ----------------- | ---- | ------------------------------------------- |
| `_hidersCounter`  | int  | Tracks total clicks by hiders during task.  |
| `_seekersCounter` | int  | Tracks total clicks by seekers during task. |

#### Public Methods

| Method                                                                             | Description                                                                                                                              |
| ---------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
| `override Task ExecuteAsync(Lobby lobby)`                                          | Starts a 10-second timer for the clicking race, then broadcasts a timeout update to players.                                             |
| `override Task EndTask(Lobby lobby, HashSet<PlayerGameSession> respondedSessions)` | Aggregates player click counts, determines winner or tie, broadcasts results, and grants abilities to MVPs. Clears task state afterward. |
| `void ResetCounters()`                                                             | Resets the internal counters for hiders and seekers to zero.   |                                                               
#### Behavior Notes

* Task runs asynchronously and waits 10 seconds for players to click.
* On completion, evaluates results based on player responses.
* Grants abilities to top hider or seeker depending on the winner.
* Handles ties gracefully by not awarding abilities.
* Clears all task updates and resets counters after finishing.

---

### **IAbility**

This interface defines the blueprint for all player abilities in the game, specifying their type, how they are applied during a game session, their usability state, and resource cleanup.

**Purpose:** Defines the contract for player abilities, specifying their type, usage conditions, effect application, and disposal.

#### Methods

| Member                                                                        | Type                        | Description                                                                          |
| ----------------------------------------------------------------------------- | --------------------------- | ------------------------------------------------------------------------------------ |
| `AbilityType Type { get; }`                                                   | Property                    | Gets the type/category of the ability.                                               |
| `Task ApplyEffectAsync(GameSession session, PlayerGameSession playerSession)` | Method                      | Applies the ability’s effect asynchronously to the given player in the game session. |
| `bool CanBeUsed { get; }`                                                     | Property                    | Indicates whether the ability is currently usable.                                   |
| `void Dispose()`                                                              | Method (from `IDisposable`) | Cleans up any resources when the ability is no longer needed.                        |

#### Enum: `AbilityType`

| Value        | Description                                            |
| ------------ | ------------------------------------------------------ |
| `GainPing`   | Ability related to gaining a ping effect or advantage. |
| `SwapQr`     | Ability that swaps QR codes or similar identifiers.    |
| `HiderSound` | Ability that triggers a sound effect for hiders.       |
| `HidePing`   | Ability to hide or suppress ping visibility.           |

---

### **HidePingAbility.cs & others**

Implements an ability that temporarily hides the player during a ping action in the game session.

**Purpose:** grants a player the power to become invisible during a ping, preventing them from being detected.

#### Properties

| Property    | Type          | Description                                                                          |
| ----------- | ------------- | ------------------------------------------------------------------------------------ |
| `Type`      | `AbilityType` | Returns `AbilityType.HidePing`, identifying the ability type.                        |
| `CanBeUsed` | `bool`        | Indicates whether the ability is currently usable. *(Note: never set in code above)* |

#### Methods

| Method                                                                        | Description                                                                               |
| ----------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------- |
| `Task ApplyEffectAsync(GameSession session, PlayerGameSession playerSession)` | Temporarily sets the player as invisible, triggers ping handling, then resets visibility. |
| `void Dispose()`                                                              | Removes this ability from the player if it is still held, and logs disposal.              |


The files `GainPingAbility.cs`, `HiderSoundAbility.cs`, and `SwapQrAbility.cs` are to be used if something needs to be sent to the server to perform an ability. Otherwise, they only use the Flutter frontend to execute them.

---

### **PlayerHandler.cs**

**Purpose:** manages WebSocket-based player operations such as login, socket assignment, and online state. It registers handlers that respond to client-initiated WebSocket messages related to player lifecycle events.

#### **Method:** `Register(WebSocketActionDispatcher dispatcher)`

Registers player-related WebSocket actions with the provided dispatcher.

#####  Registered WebSocket Actions

| Action Name    | Parameters                               | Description                                                                                                                       |
| -------------- | ---------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------- |
| `login_player` | `deviceId` (string), `username` (string) | Logs in an existing player or creates a new one. Updates their socket and online status, and returns a registration confirmation. |

#### Additional Notes

* `login_player` ensures that if a player is already registered, it reuses the player record and updates the socket connection.
* If a player is not found, a new player object is created and stored.
* The handler sets the player's online status and confirms registration via a `player_registered` message.

---

### **LobbyHandler.cs**

**Purpose:** `LobbyHandlers` class is responsible for handling WebSocket messages related to multiplayer lobby management, including creation, joining, exiting, and deletion of lobbies. It registers action handlers that interact with `LobbyManager`, `PlayerManager`, and message-sending services to maintain real-time lobby state across clients.

#### **Method:** `Register(WebSocketActionDispatcher dispatcher)`

Registers the WebSocket handlers for lobby lifecycle operations.

##### Registered WebSocket Actions

| Action Name    | Parameters                                                                    | Description                                                                                                        |
| -------------- | ----------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------ |
| `create_lobby` | `username` (string), `lobbyId` (string), `role` (string)                      | Creates a new lobby and adds the player to it. Sets the player as host if the lobby is empty.                      |
| `join_lobby`   | `username` (string), `lobbyId` (string), `nickname` (string), `role` (string) | Joins an existing lobby. If the lobby is empty, the joining player becomes the host.                               |
| `exit_lobby`   | `username` (string), `lobbyId` (string)                                       | Removes the player from the specified lobby. Notifies all lobby members and deletes the lobby if it becomes empty. |
| `delete_lobby` | `lobbyId` (string)                                                            | Deletes a lobby and notifies all remaining players that it has been removed.                                       |

**Helper Method:**

`AddPlayerToLobbyProcedure(...)`

Handles internal logic for adding a player to a lobby:

* Assigns host role if needed.
* Updates player nickname.
* Notifies the client and all lobby members.
* Deletes the previous lobby if the player switched lobbies and it became empty.

#### Behavior Notes

* If `create_lobby` is called with an existing lobby ID, it notifies the client of the failure using `ErrorMessageAsync`.
* `join_lobby` checks whether the player is the first one and assigns host status accordingly.
* `exit_lobby` ensures that all clients are updated and removes the player from the global manager.
* `delete_lobby` cleanly removes all data and notifies clients before deletion.
* Host reassignment is handled if the player leaving was the only one or the host.
* PlayerManager and LobbyManager maintain synchronization between player presence and lobby state.

---

### **GameHandler.cs**

**Purpose:** the `GameHandlers` class defines WebSocket message handlers that control the core gameplay lifecycle. This includes starting the game, managing player movement and actions, handling task progress, managing player eliminations, and using special abilities.

#### **Method:** `Register(WebSocketActionDispatcher dispatcher)`

Registers a suite of WebSocket actions related to the in-game logic and event flow.

##### Registered WebSocket Actions

| Action Name               | Parameters                                                              | Description                                                                                  |
| ------------------------- | ----------------------------------------------------------------------- | -------------------------------------------------------------------------------------------- |
| `start_game`              | `lobbyId` (string)                                                      | Initializes and starts the game session for a lobby. Notifies players and begins logic loop. |
| `set_map_center`          | `lobbyId` (string), `latitude` (double), `longitude` (double)           | Sets the map center point for a game session.                                                |
| `ping_request`            | `username` (string), `lobbyId` (string)                                 | Allows a player to request a ping location update (e.g., for seekers to find hiders).        |
| `update_position`         | `username` (string), `lobbyId` (string), `lat` (double), `lon` (double) | Updates the real-time position of a player in the game world.                                |
| `start_task`              | `username` (string), `lobbyId` (string)                                 | Starts an in-game task for the specified lobby.                                              |
| `update_task`             | `username` (string), `lobbyId` (string), `payload` (JSON)               | Sends updates about task progress, marking parts complete inside `PlayerGameSession`.        |
| `player_eliminated`       | `username` (string), `lobbyId` (string)                                 | Flags a player as eliminated and removes them from active game participation.                |
| `make_hiders_phone_sound` | `lobbyId` (string)                                                      | Sends a command to all hiders to trigger their phone sound (e.g., reveal their position).    |
| `use_ability`             | `username` (string), `lobbyId` (string), `ability` (string)             | Activates a specified player ability (like `HidePing`, `GainPing`, etc.) if available.       |

#### Behavior Notes

* `start_game` links a `Lobby` to a new `GameSession` and begins all server-side game logic.
* Location handling (`update_position`) is crucial for real-time updates, like hiding/seeking logic or geolocation-based events.
* Abilities are dynamic and retrieved via `GetAbilityFromString()` to ensure extensibility.
* Task updates use a flexible payload model allowing any task name and update format.
* Hider noise logic (`make_hiders_phone_sound`) is broadcast-style messaging, likely for special reveal moments.

---

### **PlayerManager.cs**

This singleton service manages all connected players in memory, facilitating their creation, lookup, WebSocket updates, and synchronization with the lobby system and database.

**Purpose:** centralizes the logic for managing player lifecycles, their WebSocket connections, and lobby associations to maintain a consistent multiplayer game state.

#### Public Methods

| Function                                                                                  | Description                                                                                  | Parameters                                                                                                                                 | Return Value          |
| ----------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------ | --------------------- |
| `CreatePlayer(string deviceId, string username, WebSocket socket)`                        | Creates a new player, stores it in memory, and inserts it into the database.                 | `deviceId` – Device identifier<br>`username` – Player's name<br>`socket` – Player's WebSocket connection                                   | `Player`              |
| `AddPlayerToLobby(Player player, Lobby lobby, string nickname, bool isHost, string role)` | Adds a player to a lobby or switches them between lobbies. Updates nickname, host, and role. | `player` – Player to add<br>`lobby` – Target lobby<br>`nickname` – Display name<br>`isHost` – Is player host?<br>`role` – "hider"/"seeker" | `string`              |
| `RemovePlayerFromLobby(Player player, Lobby lobby)`                                       | Removes a player from a lobby, reassigns host if needed, and deletes empty lobbies.          | `player` – Player to remove<br>`lobby` – Lobby to remove from                                                                              | `void`                |
| `RemovePlayer(string username)`                                                           | Removes a player from memory and deletes from database.                                      | `username` – Player’s username                                                                                                             | `void`                |
| `UpdatePlayerSocket(string id, WebSocket newSocket)`                                      | Replaces a player’s WebSocket connection with a new one.                                     | `id` – Player ID<br>`newSocket` – New WebSocket                                                                                            | `void`                |
| `LoginPlayer(string username)`                                                            | Marks a player as online in the database.                                                    | `username` – Player’s username                                                                                                             | `void`                |
| `GetPlayer(string id)`                                                                    | Retrieves a player by ID.                                                                    | `id` – Player ID                                                                                                                           | `Player?`             |
| `GetPlayerByName(string username)`                                                        | Retrieves a player by name.                                                                  | `username` – Player’s username                                                                                                             | `Player?`             |
| `IsPlayerInLobby(Player player)`                                                          | Returns the lobby ID the player is currently in, if any.                                     | `player` – The player object                                                                                                               | `string`              |
| `FindPlayerWithSocket(WebSocket socket)`                                                  | Finds the player associated with a specific WebSocket.                                       | `socket` – WebSocket connection                                                                                                            | `Player?`             |
| `PrintPlayers()`                                                                          | Prints the names of all players in memory to the console.                                    | *(none)*                                                                                                                                   | `void`                |
| `GetAllPlayers()`                                                                         | Returns a list of all players currently tracked in memory.                                   | *(none)*                                                                                                                                   | `IEnumerable<Player>` |


---

### **LobbyManager.cs**

This singleton service manages all active game lobbies, handling their creation, retrieval, deletion, and player membership management.

**Purpose:** maintains the collection of lobbies in memory and synchronizes lobby state with the database to support multi-player game sessions.

#### Public Methods

| Function                                 | Description                                                               | Parameters                                    | Return Value                  |
| ---------------------------------------- | ------------------------------------------------------------------------- | --------------------------------------------- | ----------------------------- |
| `CreateLobby(string lobbyId)`            | Creates a new lobby if it doesn’t exist and inserts it into the database. | `lobbyId` – Unique lobby identifier           | `Lobby` (or `null` if exists) |
| `AddLobby(string name, Lobby lobby)`     | Adds or updates a lobby in the manager's internal dictionary.             | `name` – Lobby ID<br>`lobby` – Lobby instance | `void`                        |
| `GetLobby(string lobbyId)`               | Retrieves a lobby by its ID.                                              | `lobbyId` – Lobby ID                          | `Lobby?`                      |
| `IsLobbyEmpty(string lobbyId)`           | Checks if the lobby has no players.                                       | `lobbyId` – Lobby ID                          | `bool`                        |
| `IsHost(string username)`                | Checks if the given username is a host in any lobby.                      | `username` – Player's username                | `bool`                        |
| `DeleteLobby(Lobby lobby)`               | Deletes a lobby from the database and manager if it exists.               | `lobby` – Lobby instance                      | `Lobby` (or `null`)           |
| `DeleteEmptyLobbies()`                   | Removes all lobbies that currently have no players.                       | *(none)*                                      | `void`                        |
| `DeletePlayerFromLobby(string username)` | Removes a player from any lobby they belong to in memory.                 | `username` – Player's username                | `void`                        |


---

### **AbilityManager.cs**

This static utility class manages assigning and granting abilities to players based on their roles (`hider` or `seeker`) and creates ability instances as needed.

**Purpose:** provides role-specific ability assignment logic and interfaces with messaging services to notify players of newly granted abilities.

#### Public Static Methods

| Function                                                           | Description                                                                    | Parameters                                                            | Return Value |
| ------------------------------------------------------------------ | ------------------------------------------------------------------------------ | --------------------------------------------------------------------- | ------------ |
| `GrantRandomAbility(Player player)`                                | Grants a random ability to the player based on their role and notifies them.   | `player` – The player receiving the ability                           | `void`       |
| `GrantSeekerAbility(Player player, PlayerGameSession gameSession)` | Grants a random seeker-specific ability and updates the player's game session. | `player` – Target seeker player<br>`gameSession` – Their game session | `void`       |
| `GrantHiderAbility(Player player, PlayerGameSession gameSession)`  | Grants a random hider-specific ability and updates the player's game session.  | `player` – Target hider player<br>`gameSession` – Their game session  | `void`       |


---

### **PlayerMessageSender.cs**

This class provides methods to send specific WebSocket messages to players based on in-game events.

**Purpose:** facilitates sending targeted messages to players, such as triggering sounds on hiders’ devices.

#### Public Static Methods

| Function                        | Description                                                                     | Parameters                              | Return Value |
| ------------------------------- | ------------------------------------------------------------------------------- | --------------------------------------- | ------------ |
| `SendMakeNoise(string lobbyId)` | Sends a "make\_sound" message to all hider-role players in the specified lobby. | `lobbyId` – The lobby identifier string | `Task`       |

---

### **LobbyMessageSender**

This static class provides methods to send WebSocket messages related to lobby events and player updates within a lobby.

**Purpose:** handles communication for lobby lifecycle events, player joins/leaves, host changes, player lists, and error notifications.

#### Public Static Methods

| Function                                                          | Description                                                                                   | Parameters                                                                                                          | Return Value |
| ----------------------------------------------------------------- | --------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------- | ------------ |
| `JoinedAsync(Lobby lobby, Player player, bool asHost)`            | Sends a "lobby\_joined" message to a player when they join a lobby.                           | `lobby` – Target lobby<br>`player` – Player who joined<br>`asHost` – Whether the player is the host                 | `Task`       |
| `BroadcastPlayerJoinedAsync(Lobby lobby, Player player)`          | Broadcasts a "new\_player\_joined" message to all players in a lobby when a new player joins. | `lobby` – Target lobby<br>`player` – New player joined                                                              | `Task`       |
| `SetNewHost(Player player)`                                       | Sends a "new\_host" message to notify a player that they are now the lobby host.              | `player` – Player who is the new host                                                                               | `Task`       |
| `BroadcastPlayerList(Lobby lobby)`                                | Broadcasts the current list of players in the lobby to all members.                           | `lobby` – Target lobby                                                                                              | `Task`       |
| `LeaveAsync(Lobby lobby, Player player)`                          | Sends a "leave\_lobby" message to a player who is leaving the lobby.                          | `lobby` – Lobby being left<br>`player` – Player leaving                                                             | `Task`       |
| `DeletedLobby(Lobby lobby)`                                       | Broadcasts a "lobby\_deleted" message to all players in the lobby when the lobby is deleted.  | `lobby` – Lobby being deleted                                                                                       | `Task`       |
| `ErrorMessageAsync(string lobbyId, Player player, int errorCode)` | Sends an error message to a player based on the provided error code.                          | `lobbyId` – Affected lobby ID<br>`player` – Target player<br>`errorCode` – Numeric error code identifying the error | `Task`       |

#### Error Codes in `ErrorMessageAsync`

| Code  | Meaning                 |
| ----- | ----------------------- |
| 1     | Error creating lobby    |
| 2     | Player already in lobby |
| 3     | Lobby does not exist    |
| Other | Generic error           |

---

### **GameMessageSender.cs**

This static class provides methods to send WebSocket messages related to game events, including game start/end, location requests, task updates, and player elimination notifications.

**Purpose:** facilitates communication of game state changes and player-specific updates during gameplay.

#### Public Static Methods

| Function                                                        | Description                                                          | Parameters                                                                    | Return Value |
| --------------------------------------------------------------- | -------------------------------------------------------------------- | ----------------------------------------------------------------------------- | ------------ |
| `SendGameStarted(Lobby lobby)`                                  | Broadcasts a "game\_started" message to all players in the lobby.    | `lobby` – Target lobby                                                        | `Task`       |
| `RequestHidersLocation(Lobby lobby)`                            | Broadcasts a "location\_request" message to all hiders in the lobby. | `lobby` – Target lobby                                                        | `Task`       |
| `RequestPlayersLocation(List<Player> players)`                  | Sends a "location\_request" message to a specific list of players.   | `players` – List of players to request location from                          | `Task`       |
| `SendPingToSeekers(Lobby lobby, List<Player> updatedLocations)` | Broadcasts location updates to seekers in the lobby.                 | `lobby` – Target lobby<br>`updatedLocations` – Players with updated locations | `Task`       |
| `SendGameEnded(Lobby lobby)`                                    | Broadcasts a "game\_ended" message to all players in the lobby.      | `lobby` – Target lobby                                                        | `Task`       |
| `SendTimeUpdate(Lobby lobby, TimeSpan time)`                    | Broadcasts a "time\_update" message with the current game time.      | `lobby` – Target lobby<br>`time` – Remaining or elapsed game time             | `Task`       |

#### Task-Related Messages

| Function                                           | Description                                                          | Parameters                                             | Return Value |
| -------------------------------------------------- | -------------------------------------------------------------------- | ------------------------------------------------------ | ------------ |
| `BroadcastTask(Lobby lobby, GameTask task)`        | Broadcasts "task\_started" message for a game task to lobby players. | `lobby` – Target lobby<br>`task` – Task starting       | `Task`       |
| `BroadcastUpdateTask(Lobby lobby, object payload)` | Broadcasts a "task\_update" message with task progress or state.     | `lobby` – Target lobby<br>`payload` – Task update data | `Task`       |
| `BroadcastTaskResult(Lobby lobby, string winners)` | Broadcasts a "task\_result" message announcing winners.              | `lobby` – Target lobby<br>`winners` – Winner(s)        | `Task`       |

#### Player Elimination and Ability Messages

| Function                                                         | Description                                                            | Parameters                                                  | Return Value |
| ---------------------------------------------------------------- | ---------------------------------------------------------------------- | ----------------------------------------------------------- | ------------ |
| `BroadcastEliminatedPlayer(Lobby lobby, Player player)`          | Broadcasts an "eliminated\_player" message to all lobby players.       | `lobby` – Target lobby<br>`player` – Eliminated player      | `Task`       |
| `SendEliminatedPlayer(Player player)`                            | Sends a "current\_player\_eliminated" message to a specific player.    | `player` – Player being eliminated                          | `Task`       |
| `NotifyAbilityGainAsync(Player player, AbilityType abilityType)` | Sends a "gained\_ability" message to notify a player of a new ability. | `player` – Target player<br>`abilityType` – Granted ability | `Task`       |