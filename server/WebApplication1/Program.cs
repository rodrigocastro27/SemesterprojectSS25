using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WebApplication1;

var builder = WebApplication.CreateBuilder();
var app = builder.Build();
app.UseWebSockets();

var lobbyManager = new LobbyManager();

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var socket = await context.WebSockets.AcceptWebSocketAsync();
        var buffer = new byte[1024 * 4];

        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var doc = JsonDocument.Parse(msg);
                var action = doc.RootElement.GetProperty("action").GetString();

                if (action == "join_lobby")
                {
                    var name = doc.RootElement.GetProperty("name").GetString();
                    var lobbyId = doc.RootElement.GetProperty("lobbyId").GetString();

                    var player = new Player(name!, socket);
                    var lobby = lobbyManager.GetOrCreateLobby(lobbyId!);
                    await lobby.AddPlayerAsync(player);
                }
            }
        }
    }
});

app.Run("http://0.0.0.0:5000");