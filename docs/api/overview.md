# Messaging System Overview

This document explains the messaging system used in the project for communication between the Flutter client and the C# backend server via WebSockets.

## Libraries and Tools Used

|Library / Package|Purpose|
|---|---|
|`dart:io`|Handles low-level WebSocket connections via `WebSocket.connect`.|
|`dart:convert`|Encodes and decodes JSON data using `jsonEncode` and `jsonDecode`.|
|`provider`|Manages shared app state and updates UI reactively.|
|`go_router`|Handles navigation based on messages from the server.|

## Messaging Flow

### 1. Connection

- The `WebSocketService` class manages the WebSocket lifecycle.
- It connects to the server using `WebSocket.connect()` from `dart:io`.
- Automatically attempts reconnection using exponential backoff if the connection fails.

### 2. Sending Messages

- The `MessageSender` class provides high-level static methods to send messages (e.g., `joinLobby`, `startGame`).
- Each method internally calls `webSocketService.send()` which encodes the action and payload as JSON.

### 3. Receiving Messages

- Incoming messages are received by the WebSocket listener in `WebSocketService`.
- These messages are passed to the `ServerActionDispatcher`.
- The dispatcher parses the `action` field and invokes the corresponding handler function.

### 4. Handling Actions

- Action handlers are registered in `GameActions`, `LobbyActions`, and `PlayerActions`.
- Each handler updates the app state using `provider` or triggers UI updates (e.g., navigation).
- Example: `game_started`, `location_update_list`, `task_result`.

## Key Components

- `**WebSocketService**`: Core communication class, manages connection and transmission.
    
- `**MessageSender**`: Simplifies sending typed messages to the server.
    
- `**ServerActionDispatcher**`: Routes received messages to appropriate handlers.
    
- **Handlers (**`**GameActions**`**, etc.)**: Execute logic based on specific server actions.