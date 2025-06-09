using System.Net.WebSockets;
using System.Text;
using WebApplication1.Handlers;
using WebApplication1.Utils;

var app = WebApplication.Create();
app.UseWebSockets();

var dispatcher = new WebSocketActionDispatcher();

LobbyHandlers.Register(dispatcher);
PlayerHandlers.Register(dispatcher);


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

await app.RunAsync("http://0.0.0.0:5100");
