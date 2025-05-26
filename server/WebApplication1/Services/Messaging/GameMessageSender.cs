using WebApplication1.Utils;
using WebApplication1.Models;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Services.Messaging;

public static class GameMessageSender
{
    public static async Task SendGameStarted(Lobby lobby, string message)
    {
        await MessageSender.BroadcastLobbyAsync(lobby!, "game_started", new
        {
            message = message,
        });
    }
}