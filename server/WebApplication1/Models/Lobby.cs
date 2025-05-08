using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace WebApplication1.Models;

public class Lobby
{
    public string Id { get; }
    public List<Player> Players { get; } = new();
    private bool _timerRunning = false;

    public Lobby(string id)
    {
        Id = id;
    }

    public async Task AddPlayerAsync(Player player)
    {
        Players.Add(player);

        if (!_timerRunning)
        {
            _timerRunning = true;
            await StartTimerAsync();
        }
    }

    private async Task StartTimerAsync()
    {
        for (int i = 10; i >= 0; i--)
        {
            var message = JsonSerializer.Serialize(new {
                action = "timer_update",
                seconds = i
            });

            var buffer = Encoding.UTF8.GetBytes(message);
            foreach (var p in Players)
            {
                if (p.Socket.State == WebSocketState.Open)
                {
                    await p.Socket.SendAsync(
                        new ArraySegment<byte>(buffer),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    );
                }
            }

            await Task.Delay(1000);
        }
    }
}
