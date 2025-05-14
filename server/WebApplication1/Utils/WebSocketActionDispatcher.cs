using System.Net.WebSockets;
using System.Text.Json;

namespace WebApplication1.Utils;

public class WebSocketActionDispatcher
{
    private readonly Dictionary<string, Func<JsonElement, WebSocket, Task>> _handlers = new();

    public void Register(string action, Func<JsonElement, WebSocket, Task> handler) => _handlers[action] = handler;

    public async Task HandleMessage(string jsonMessage, WebSocket socket)
    {
        var doc = JsonDocument.Parse(jsonMessage);
        var root = doc.RootElement;

        if (!root.TryGetProperty("action", out var actionElem) ||
            !root.TryGetProperty("data", out var dataElem))
        {
            Console.WriteLine($"Malformed message: {jsonMessage}");
            return;
        }

        var action = actionElem.GetString();
        Console.WriteLine($"Action: {action}");

        if (action != null && _handlers.TryGetValue(action, out var handler))
        {
            await handler(dataElem, socket); // Only pass the 'data' part
        }
        else
        {
            Console.WriteLine($"Unknown action: {action}");
        }
    }

}