using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Services.Messaging;
using WebApplication1.Utils;

namespace WebApplication1.Handlers;

public static class GameHandlers
{
    public static void Register(WebSocketActionDispatcher dispatcher)
    {
        
        dispatcher.Register("start_game", async (data, socket) =>
        {
            var lobbyId = data.GetProperty("lobbyId").GetString();

            Console.WriteLine($"\n[start_game] Proceeding to start game for lobby {lobbyId}.");

            var lobby = LobbyManager.Instance.GetLobby(lobbyId!);
            
            Console.WriteLine("Notifying all players in the lobby that the game is starting.");
            await GameMessageSender.SendGameStarted(lobby!);
            
            /*START the GAME in GameSession:

                ->Instantiate a GameSession Class

                -> Link a lobby to a GameSession

                -> call tart function to kick off the logic
            */
            
            GameSession gameSession = new GameSession(lobby!);
            
            await gameSession.Start();

        });
        
        
        
        dispatcher.Register("ping_request", (data, socket) =>
        {
            var username = data.GetProperty("username").GetString();
            var lobbyId = data.GetProperty("lobbyId").GetString();

            var lobby = LobbyManager.Instance.GetLobby(lobbyId!);
            var player = PlayerManager.Instance.GetPlayer(username!);
            
            Console.WriteLine($"\n {player?.Name} requested a ping in lobby: {lobbyId}.");
            
            GameSession? session = lobby?.GetGameSession();
            
            //starts ping logic 
            if (player != null) session?.RequestPing(player);
            return Task.CompletedTask;
        });
        
        //request received when players send their original location
        dispatcher.Register("update_position", (data, socket) =>
        {
            
            var username = data.GetProperty("username").GetString();
            var lobbyId = data.GetProperty("lobbyId").GetString();
            var longitude = data.GetProperty("lon").GetDouble();
            var latitude = data.GetProperty("lat").GetDouble();

            var lobby = LobbyManager.Instance.GetLobby(lobbyId!);
            var player = PlayerManager.Instance.GetPlayer(username!);
            
            if (player == null || lobby == null) return Task.CompletedTask;
            
            GeoPosition geoPosition = new GeoPosition(latitude, longitude);
            
            player?.UpdateLocation(geoPosition);
            return Task.CompletedTask;
        });
        
        
    }
}