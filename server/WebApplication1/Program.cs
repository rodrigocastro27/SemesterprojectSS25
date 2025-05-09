using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Collections.Concurrent;
using WebApplication1;
using WebApplication1.Handlers;
using WebApplication1.Models;

var app = WebApplication.Create();
app.UseWebSockets();

var dispatcher = new WebSocketActionDispatcher();
var lobbyManager = new LobbyManager();

LobbyHandlers.Register(dispatcher, lobbyManager);
PlayerHandlers.Register(dispatcher, lobbyManager);



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
                await dispatcher.HandleMessage(msg, socket);
            }
        }

    }
    
});

await app.RunAsync("http://0.0.0.0:5000");