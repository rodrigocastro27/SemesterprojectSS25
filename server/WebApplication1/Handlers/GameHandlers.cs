using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Services.Messaging;
using WebApplication1.Utils;

namespace WebApplication1.Handlers;

public class GameHandlers
{
    public static void Register(WebSocketActionDispatcher dispatcher, LobbyManager manager)
    {
        
        //request received when player requests a ping through the pinging button
        dispatcher.Register("ping_request", (data, socket) =>
        {
            var username = data.GetProperty("username").GetString();
            var lobbyId = data.GetProperty("lobbyId").GetString();

            var lobby = LobbyManager.Instance.GetLobby(lobbyId!);
            var player = PlayerManager.Instance.GetPlayer(username!);
            
            Console.WriteLine($"\n {player?.Name} requested a ping in lobby: {lobbyId}.");
            
            //start pinging logic here

            
            GameSession? session = lobby?.GetGameSession();
            
            if (player != null) session?.RequestPing(player);
            return Task.CompletedTask;
        });
        
        //request received when players send their original location
        dispatcher.Register("location_update", (data, socket) =>
        {
            
            var username = data.GetProperty("username").GetString();
            var lobbyId = data.GetProperty("lobbyId").GetString();
            var longitude = data.GetProperty("longitude").GetDouble();
            var latitude = data.GetProperty("latitude").GetDouble();

            var lobby = LobbyManager.Instance.GetLobby(lobbyId!);
            var player = PlayerManager.Instance.GetPlayer(username!);
            
            if (player == null || lobby == null) return Task.CompletedTask;
            
            GeoPosition geoPosition = new GeoPosition(latitude, longitude);
            
            player?.UpdateLocation(geoPosition);
            return Task.CompletedTask;
        });
    }
}