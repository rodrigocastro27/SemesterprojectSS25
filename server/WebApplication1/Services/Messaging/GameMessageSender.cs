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

    public static async Task SendPingActivated(Lobby lobby, List<Player> updatedLocations, string requestingPlayerId)
    {
        var playersData = updatedLocations.Select(p => new
        {
            name = p.Name,
            id = p.Id,
            latitude = p.Position.Latitude,
            longitude = p.Position.Longitude,
        });
        
        await MessageSender.BroadcastToSeekers(lobby, "ping_activated", new
        {
            players = playersData,
            requestingPlayerId = requestingPlayerId
        });
    }
    
    public static async Task SendPingRejected(Player player)
    {
        await MessageSender.SendToPlayerAsync(player, "ping_rejected", new
        {
            reason = "Another seeker is already pinging"
        });
    }
    
    public static async Task SendPingEnded(Lobby lobby, string requestingPlayerId)
    {
        await MessageSender.BroadcastToSeekers(lobby, "ping_ended", new
        {
            requestingPlayerId = requestingPlayerId
        });
    }
    
    public static async Task SendPingCooldownEnded(Lobby lobby)
    {
        await MessageSender.BroadcastToSeekers(lobby, "ping_cooldown_ended", new { });
    }
    
    public static async Task SendGameEnded(Lobby lobby)
    {
        await MessageSender.BroadcastLobbyAsync(lobby, "game_ended", new
        {
            //who won, other relevant info
        });
    }

    #region Task Messages

    public static async Task Task1(Lobby lobby)
    {
            //communicate random task has begun
    }
    

    #endregion
    
}

