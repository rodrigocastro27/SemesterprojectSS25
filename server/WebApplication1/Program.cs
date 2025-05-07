using System.Net;
using System.Net.WebSockets;
using Fleck;

namespace WebApplication1;

public class Program
{
    public static void Main(string[] args)
    {
        var wsConnections = new List<IWebSocketConnection>();
        
        var server  = new WebSocketServer("ws://0.0.0.0:8181");
        
        server.Start(ws =>
        {
            ws.OnOpen = () =>
            {
                Console.WriteLine($"Client connected: {ws.ConnectionInfo.ClientIpAddress}");
                wsConnections.Add(ws);        
            };
            
            ws.OnMessage = message =>
            {
                Console.WriteLine($"Received: {message}");

                foreach (var webSocketConnection in wsConnections)
                {
                    webSocketConnection.Send($"Echo: {message}");
                }
            };

            ws.OnClose = () =>
            {
                Console.WriteLine($"Client disconnected: {ws.ConnectionInfo.ClientIpAddress}");
                wsConnections.Remove(ws);
            };
            
        });
        
        WebApplication.CreateBuilder(args).Build().Run();
    }
}