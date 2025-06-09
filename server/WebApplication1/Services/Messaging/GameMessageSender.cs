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
            lat = p.Position.Latitude,
            lon = p.Position.Longitude,
        });

        await MessageSender.BroadcastToSeekers(lobby, "location_update_list", new
        {
            players = playersData
        });
    }
    
    public static async Task SendGameEnded(Lobby lobby)
    {
        await MessageSender.BroadcastLobbyAsync(lobby, "game_ended", new
        {
            //who won, other relevant info
        });
    }
    
    public static async Task SendTimeUpdate(Lobby lobby, TimeSpan time)
    {
        DateTime now = DateTime.Now;
        await MessageSender.BroadcastLobbyAsync(lobby, "time_update", new
        {
            time = time,
            time_offset = now,
        });
    }
    
    #region Task Messages

    // public static async Task Task1(Lobby lobby)
    // {
    //         //communicate random task has begun
    // }
    

    #endregion
    
}