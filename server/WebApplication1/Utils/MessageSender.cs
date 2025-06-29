﻿using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WebApplication1.Models;

namespace WebApplication1.Utils
{
    public static class MessageSender
    {
        public static async Task SendAsync(WebSocket socket, string action, object data)
        {
            try
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
            catch (WebSocketException ex)
            {
                Console.WriteLine($"Failed to send message to socket: {ex.Message}");
                // Optional: mark socket for cleanup or notify manager
            }
        }

        public static async Task SendToPlayerAsync(Player player, string action, object data)
        {
            if (player.Socket != null && player.Socket.State == WebSocketState.Open)
            {
                await SendAsync(player.Socket, action, data);
            }
            else
            {
                Console.WriteLine($"Cannot send to player {player.Name} - socket not open.");
            }
        }
        
        public static async Task BroadcastToHiders(Lobby lobby, string action, object data)
        {
            foreach (var player in lobby.Players)
            {
                if (player.GetRole() == Role.hider)
                {
                    await SendToPlayerAsync(player, action, data);
                }
            }
        }

        public static async Task BroadcastToSeekers(Lobby lobby, string action, object data)
        {
            foreach (var player in lobby.Players)
            {
                if (player.GetRole() == Role.seeker)
                {
                    await SendToPlayerAsync(player, action, data);
                }
            }
        }
        
        
        
        private static async Task TrySendAsync(Player player, string action, object data)
        {
            try
            {
                await SendToPlayerAsync(player, action, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARN] Failed to send to {player.Name}: {ex.Message}");
            }
        }

        public static async Task BroadcastAsync(IEnumerable<Player> players, string action, object data)
        {
            var tasks = players
                .Where(p => p.Socket?.State == WebSocketState.Open)
                .Select(p => TrySendAsync(p, action, data));

            await Task.WhenAll(tasks);
        }

        public static Task BroadcastLobbyAsync(Lobby lobby, string action, object data)
            => BroadcastAsync(lobby.Players, action, data);

        public static Task BroadcastToHidersAsync(Lobby lobby, string action, object data)
            => BroadcastAsync(lobby.GetHidersList(), action, data);

        public static Task BroadcastToSeekersAsync(Lobby lobby, string action, object data)
            => BroadcastAsync(lobby.GetSeekersList(), action, data);
    }
}