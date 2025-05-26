using System.Net.WebSockets;
using System.Text;
using WebApplication1.Handlers;
using WebApplication1.Utils;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();
app.UseWebSockets();
SQLiteConnector.Initialize(builder.Environment.ContentRootPath);

var dispatcher = new WebSocketActionDispatcher();

LobbyHandlers.Register(dispatcher);
PlayerHandlers.Register(dispatcher);
GameHandlers.Register(dispatcher);

DataLoader.LoadAll();


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
                Console.WriteLine(msg);
                await dispatcher.HandleMessage(msg, socket);
            }
        }
    }
});

await app.RunAsync("http://0.0.0.0:5000");