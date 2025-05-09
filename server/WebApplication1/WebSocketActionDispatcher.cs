using System.Net.WebSockets;
using System.Text.Json;

namespace WebApplication1;

public class WebSocketActionDispatcher
{
    private readonly Dictionary<string, Func<JsonElement, WebSocket, Task>> _handlers = new();

    public void Register(string action, Func<JsonElement, WebSocket, Task> handler) => _handlers[action] = handler;

    public async Task HandleMessage(string jsonMessage, WebSocket socket)
    {
        var doc = JsonDocument.Parse(jsonMessage);
        var root = doc.RootElement;
        var action = root.GetProperty("action").GetString();

        if (action != null && _handlers.TryGetValue(action, out var handler))
        {
            await handler(root, socket);
        }
        else
        {
            Console.WriteLine($"Unknown action: {action}");
        }
    }
}