using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WebApplication1.Models;

namespace WebApplication1.Utils
{
    public static class MessageSender
    {
        public static async Task SendAsync(WebSocket socket, string action, object data)
        {
            var message = new
            {
                action,
                data
            };

            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);

            await socket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );
        }

        public static async Task SendToPlayerAsync(Player player, string action, object data)
        {
            if (player.Socket.State == WebSocketState.Open)
            {
                await SendAsync(player.Socket, action, data);
            }
        }
        
        public static async Task BroadcastLobbyAsync(Lobby lobby, string action, object data)
        {
            foreach (var player in lobby.Players)
            {
                if (player.Socket.State == WebSocketState.Open)
                {
                    await SendToPlayerAsync(player, action, data);
                }
            }
        }
    }
}