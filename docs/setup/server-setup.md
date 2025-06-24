# Server Setup

For the setup of the server, and the execution of the program, we used two tools: **either** VSCode **or** JetBrains Raider.

## 🔧 Requirements

| Tool | Windows | macOS | Linux |
|------|---------|-------|-------|
|.NET SDK | [.NET 8.0+](https://dotnet.microsoft.com/en-us/download) |	Same | Same |
| VS Code | [Download](https://code.visualstudio.com/) | Same | Same |
| JetBrains Rider | [Download](https://www.jetbrains.com/rider/#) (**with license**) | Same | Same |
| SQLite | Preinstalled or via [winget/choco] | ``brew install sqlite`` | ``sudo apt install sqlite3`` |

Check your setup:

```bash
dotnet --version
sqlite3 --version
```

## 🌐 Exposing the Server with ngrok
In development, ngrok is used to tunnel the local backend (running on `localhost:5000`) to a public URL.

### 🔧 Step 1: Install ngrok
OS	Installation Command
* Windows: Download from https://ngrok.com/download
* macOS: ``brew install ngrok/ngrok/ngrok``
* Linux: ``snap install ngrok`` or download from ngrok.com

### 🔑 Step 2: Authenticate ngrok
* Sign up at https://dashboard.ngrok.com
* Get your auth token: Go to **Identity & Access** > **Authtokens** and copy the ID token.
* Authenticate: 
    ```bash
    ngrok config add-authtoken <YOUR_TOKEN_HERE>
    ```

### 🌍 Step 3: Start ngrok tunnel
```bash
ngrok http 5000
```
The port is usually `5000` or `5001`. Just make sure it is the one indicated at the end of the `Program.cs` file:

```cs
await app.RunAsync("http://0.0.0.0:5000");
```

⚠️ Make sure ngrok is at your Environment Variables PATHs. Otherwise, open the `ngrok.exe` and run the same command.

You'll see output like:

```nginx
Session Status                online
Account                       [youruseremail@example.com] (Plan: Free)
Version                       3.23.1
Region                        Europe (eu)  
Latency                       17ms
Web Interface                 http://127.0.0.1:4040
Forwarding                    https://02ab-193-170-134-240.ngrok-free.app -> http://localhost:5000
Connections                   ttl     opn     rt1     rt5     p50     p90
                              0       0       0.00    0.00    0.00    0.00
```
Copy
Edit
Forwarding                    https://a1b2c3d4.ngrok.io -> http://localhost:5000


Then, in `main.dart`, use the given address in the following line of code:

```dart
// The part: 02ab-193-170-134-240.ngrok-free.app
webSocketService.connect('wss://02ab-193-170-134-240.ngrok-free.app/ws');
```

## 🚀 Run the Server

Finally, you can start the server in the terminal by:

```bash
cd ../semester_project/server/WebApplication1   # change to project directory
dotnet watch    # execute server
```

If the server starts correctly, it should open a page in the browser. We used `watch` instead of `run` it is easier for development (restarts the app each time a file is changed).