using WebApplication1.Utils;
using WebApplication1.Models;

namespace WebApplication1.Services.Messaging;

public static class GameMessageSender
{
    public static async Task SendGameStarted(Lobby lobby, string message)
    {
        await MessageSender.BroadcastLobbyAsync(lobby!, "game_started", new
        {
            action = "started",
        });
    }
}