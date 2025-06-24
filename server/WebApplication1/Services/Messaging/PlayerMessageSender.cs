using WebApplication1.Models;
using WebApplication1.Utils;

namespace WebApplication1.Services.Messaging;

public class PlayerMessageSender
{
    public static async Task SendMakeNoise(string lobbyId)
    {
        var lobby = LobbyManager.Instance.GetLobby(lobbyId!);
        foreach (var player in lobby!.Players)
        {
            if (player.GetRole() == Role.hider)
            {
                await MessageSender.SendToPlayerAsync(player, "make_sound", new
                {
                });
            }
        }
    }
}