# Overview

## Messaging

The messaging system in this program facilitates real-time communication between the server and connected Flutter clients using WebSockets. It enables the exchange of structured JSON messages to synchronize game state, player actions, and lobby updates.

### Key Concepts

* **WebSocket Communication:**
  All messages are sent as JSON over WebSocket connections, allowing instant bidirectional data flow between server and clients.

* **MessageSender Utility:**
  This core static utility provides low-level methods to send JSON messages to individual players or broadcast to entire lobbies, optionally filtered by player roles (e.g., seekers or hiders).

* **Message Types and Scopes:**
  Different static message sender classes handle specific domains:

  * **LobbyMessageSender:** Manages lobby lifecycle events such as joining, leaving, host changes, and error notifications.
  * **PlayerMessageSender:** Handles player-specific messages, e.g., triggering sounds or direct player notifications.
  * **GameMessageSender:** Deals with game state events like game start/end, location requests, task updates, and ability gains.

* **Incoming Messages:**
  The server receives WebSocket messages from the Flutter front end representing player actions (e.g., joining a lobby, moving location, using abilities). These are processed to update game state and then broadcast relevant updates back to clients.

### Message Format

All messages follow a simple JSON structure containing:

* An `"action"` string specifying the event type.
* A `"data"` object carrying relevant payload information.

Example:

```json
{
  "action": "player_list",
  "data": {
    "players": [
      { "name": "Alice", "role": "hider", "nickname": "Al" },
      { "name": "Bob", "role": "seeker", "nickname": "Bobby" }
    ]
  }
}
```

### Workflow Summary

1. **Flutter Client Sends:**
   The client sends JSON WebSocket messages indicating player intents or responses.

2. **Server Processes:**
   The server interprets these messages, updates game/lobby state, interacts with the database as needed, and triggers relevant server-side logic.

3. **Server Responds:**
   The server uses the appropriate message sender class to broadcast or send updated state and event notifications back to the relevant players or lobbies.

4. **Clients Update UI:**
   The Flutter front end receives these JSON messages and updates the UI accordingly (e.g., player lists, game timers, ability notifications).

---

This messaging design ensures efficient and organized real-time synchronization between server and Flutter clients, enabling smooth multiplayer game experiences.

## Multiplayer Lobby

The multiplayer lobby system in this application coordinates how players connect, interact, and play together within shared game sessions. It involves three main components:

* **Flutter Frontend:** The client interface where players join lobbies, interact with other players, and play the game.
* **Server Program:** Manages lobby state, players, roles, and communication.
* **Database:** Persists lobby information, player data, and game progress.

---

### Lobby Creation and Joining

* When a player attempts to join or create a lobby via the Flutter frontend, the request is sent to the server.
* The server’s `LobbyManager` handles lobby creation by checking if the lobby already exists in its internal collection and the database.
* If the lobby is new, it inserts the lobby into the database and updates the internal manager.
* When a player joins an existing lobby, the server verifies if the player is already in another lobby. If so, it manages their transition between lobbies, updating both the in-memory state and the database.
* Each player’s lobby membership and role (e.g., host, seeker, hider) are tracked and persisted.

---

### Player Management in Lobbies

* Players are managed by a singleton `PlayerManager`, which holds active player instances linked by username.
* Player objects contain the current WebSocket connection, role, nickname, and lobby membership.
* When a player joins a lobby, the server updates the database to reflect this and adds the player to the lobby’s internal player list.
* The `PlayerManager` supports updating player sockets for reconnections and removing players upon disconnection or logout.

---

### Reconnection Handling and the `online` Variable

* Players may disconnect unexpectedly due to network issues or app closure.
* When a disconnection occurs, the server updates the player's `online` status in the database to `false` but **does not remove them from the lobby or reset gameplay state**.
* The player’s state (role, position, game progress) remains preserved on the server and in the database.
* Upon reconnection from the Flutter frontend, the client reestablishes a WebSocket connection.
* The server updates the player’s socket reference in `PlayerManager` and sets the `online` status back to `true`.
* This process allows seamless reconnection without restarting the game or losing player progress.
* The server synchronizes any necessary game state or lobby data back to the player so their client can resume the session smoothly.

---

### Host and Lobby Management

* Each lobby has one player designated as the host.
* If the host disconnects or leaves, the server selects a new host from remaining players automatically.
* Host changes are updated in both the internal state and database.
* If a lobby becomes empty (all players leave or disconnect), the server deletes the lobby from memory and the database to conserve resources.

---

### Player Removal and Role Changes

* Players can leave lobbies voluntarily or be removed by the host.
* The server updates the lobby and database accordingly.
* Player roles (e.g., seeker or hider) can be changed or assigned when joining a lobby or during gameplay.
* Role changes are communicated to all relevant players via messaging services.

---

### Data Persistence and Synchronization

* The database keeps authoritative records of:

  * Lobbies (IDs and metadata)
  * Players and their online/offline status
  * Player lobby membership and roles
  * Ongoing game states and tasks as needed
* The server ensures that in-memory state and database records are kept consistent during all lobby operations, including player joins, leaves, disconnections, and reconnections.

## Game Logic

The core gameplay revolves around two opposing roles: **seekers** and **hiders**. The game logic manages player states, actions, and interactions to create an engaging multiplayer experience. Key components include **tasks**, **pings**, and **abilities**, each integrated into the gameplay loop and coordinated via the server and Flutter frontend.

### Tasks

**Tasks** are interactive objectives or challenges that players must complete during the game.

* **Purpose:**
  Tasks provide goals that both seekers and hiders can engage with to influence the outcome of the game. For example, tasks may involve finding specific items, solving puzzles, or completing mini-games within the lobby context.

* **How tasks work:**

  * When a task starts, the server broadcasts a `task_started` message to all players, informing them about the active task.
  * Players interact with the task through the frontend, sending updates to the server about their progress.
  * The server receives task updates, validates them, and broadcasts these updates (`task_update`) to keep all players in sync.
  * Once a task is completed, the server broadcasts the results (`task_result`) showing winners or consequences of the task completion.
  * Task results can impact the ongoing game, such as revealing player positions, granting bonuses, or altering roles.

* **Integration with gameplay:**
  Tasks add strategic layers and pacing to the game. Completing tasks may provide advantages or trigger events that affect seekers and hiders differently, making the gameplay dynamic and engaging.

---

### Pings

**Pings** represent location updates or signals used primarily by seekers to track hiders or by hiders to gain strategic information.

* **Purpose:**
  Pings are a mechanic for players, especially seekers, to receive location data about opponents or points of interest. They help balance the game by giving seekers limited but useful tracking information.

* **How pings work:**

  * The server collects player location data, focusing on hiders’ positions when a ping request is asked from the flutter end.
  * These locations are sent as a batch (`location_update_list`) to seekers via WebSocket messages.
  * Seekers receive pings as partial, delayed, or noisy information to maintain game tension.
  * Hiders may have abilities that affect the accuracy or frequency of these pings, introducing gameplay variation.
  * Players can also request location updates (`location_request`) to trigger immediate pings if needed.

* **Integration with gameplay:**
  Pings are critical in the cat-and-mouse dynamic of seekers versus hiders. They provide seekers with clues while giving hiders opportunities to evade or use abilities to disrupt these signals.

---

### Abilities

**Abilities** are special powers granted to players based on their role, adding unique strategic options.

* **Purpose:**
  Abilities enrich gameplay by giving players role-specific advantages or tools to influence the match. They make each round more varied and customizable.

* **How abilities work:**

  * The `AbilityManager` selects abilities from predefined lists according to player roles.
  * Abilities might include hiding player pings, swapping QR codes, generating sound distractions, or gaining ping information.
  * Players use abilities by triggering corresponding commands or UI actions in the Flutter frontend.
  * The server manages the effect of abilities, applying game logic changes such as temporarily disabling location broadcasts or changing player visibility.
  * Ability activations and gains are broadcasted to players so all clients can update UI and gameplay state accordingly.

* **Integration with gameplay:**
  Abilities create tactical depth and diversity, enabling players to devise creative strategies. For instance, a hider might activate a stealth ability to avoid detection, while a seeker might enhance their tracking capabilities.

---

### Game Flow Integration

* The game begins with lobby setup and role assignments.
* Tasks are started, updated, and completed throughout gameplay, driving player engagement and interaction.
* Pings provide ongoing location updates, allowing seekers to track hiders while hiders attempt to evade.
* Abilities are granted throughout the game, offering dynamic interactions and counters.
* The server continuously synchronizes all events via WebSocket messages to keep all clients (Flutter frontends) in sync.
* Game state updates — such as task progress, ping broadcasts, and ability usage — are sent reliably and asynchronously.
* When the game ends, the server broadcasts final results, concluding the session.