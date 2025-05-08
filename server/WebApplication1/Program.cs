using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Collections.Concurrent;

var app = WebApplication.Create();
app.UseWebSockets();

var lobbies = new ConcurrentDictionary<string, WebSocket>();

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var socket = await context.WebSockets.AcceptWebSocketAsync();

        var buffer = new byte[1024];
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
                    var playerName = doc.RootElement.GetProperty("name").GetString();
                    lobbies[playerName] = socket;

                    // Send timer start event
                    for (int i = 10; i >= 0; i--)
                    {
                        var timerMsg = JsonSerializer.Serialize(new {
                            action = "timer_update",
                            seconds = i
                        });
                        await socket.SendAsync(
                            Encoding.UTF8.GetBytes(timerMsg),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None
                        );
                        await Task.Delay(1000);
                    }
                }
            }
        }
    }
});

await app.RunAsync("http://0.0.0.0:5000");