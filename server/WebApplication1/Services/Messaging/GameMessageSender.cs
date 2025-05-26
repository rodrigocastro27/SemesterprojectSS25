using WebApplication1.Utils;
using WebApplication1.Models;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Services.Messaging;

public static class GameMessageSender
{
    public static async Task SendGameStarted(Lobby lobby)
    {
        await MessageSender.BroadcastLobbyAsync(lobby!, "game_started", new
        {
        });
    }
    public static async Task RequestHidersLocation(Lobby lobby)
    {
        await MessageSender.BroadcastToHiders(lobby, "location_request", new
        {
            //idk what else to send
        });
    }

    public static async Task SendPingToSeekers(Lobby lobby, List<Player> updatedLocations)
    {
        var playersData = updatedLocations.Select(p => new
        {
            name = p.Name,
            id = p.Id,
            latitude = p.Position.Latitude,
            longitude = p.Position.Longitude,
        });

        await MessageSender.BroadcastToSeekers(lobby, "player_location_list", new
        {
            players = playersData
        });
    }
    
    public static async Task SendGameEnded(Lobby lobby, string message)
    {
        await MessageSender.BroadcastLobbyAsync(lobby, "game_ended", new
        {
            //who won, other relevant info
        });
    }
    
}