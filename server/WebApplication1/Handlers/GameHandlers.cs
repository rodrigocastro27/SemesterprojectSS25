using Microsoft.Net.Http.Headers;
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

        dispatcher.Register("set_map_center", (data, socket) =>
        {
            var lobbyId = data.GetProperty("lobbyId").GetString();
            var lat = data.GetProperty("latitude").GetDouble();
            var lon = data.GetProperty("longitude").GetDouble();

            var lobby = LobbyManager.Instance.GetLobby(lobbyId!);
            GameSession gameSession = lobby!.GetGameSession()!;
            gameSession.SetMapCenter(lat, lon);
            Console.WriteLine($"SET MAP CENTER TO ({lat},{lon})");

            return Task.CompletedTask;
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


        dispatcher.Register("start_task", async (data, socket) =>
        {
            Console.WriteLine("Starting request to start task...");
            var username = data.GetProperty("username").GetString();
            var lobbyId = data.GetProperty("lobbyId").GetString();

            var lobby = LobbyManager.Instance.GetLobby(lobbyId!);
            var gameSession = lobby!.GetGameSession()!;

            await gameSession.StartTask(lobby);   // assuming game session and lobby are not null
        });


        dispatcher.Register("update_task", (data, socket) =>
        {
            Console.Write("\nReceived update from task.");
            var username = data.GetProperty("username").GetString();
            var lobbyId = data.GetProperty("lobbyId").GetString();
            var payload = data.GetProperty("payload");

            var taskName = payload.GetProperty("taskName").GetString();
            var update = payload.GetProperty("update");
            var type = update.GetProperty("type").GetString();
            var info = update.GetProperty("info");

            var lobby = LobbyManager.Instance.GetLobby(lobbyId!);
            var player = PlayerManager.Instance.GetPlayerByName(username!);
            var gameSession = lobby!.GetGameSession();

            // Find PlayerSession by username or socket
            var playerSession = gameSession!.GetPlayerGameSession(player!);

            if (playerSession != null)
            {
                playerSession.MarkUpdateReceived(taskName!, info); // set a flag inside PlayerSession
            }

            return Task.CompletedTask;
        });

        dispatcher.Register("player_eliminated", (data, socket) =>
        {
            var username = data.GetProperty("username").GetString();
            var lobbyId = data.GetProperty("lobbyId").GetString();
            
            var lobby = LobbyManager.Instance.GetLobby(lobbyId!);
            var player = PlayerManager.Instance.GetPlayer(username!);

            if (player == null || lobby == null) return Task.CompletedTask;
            
            Console.WriteLine($"Player is eliminated: {player.Name}");
            
            var session = lobby?.GetGameSession();
            session?.EliminatePlayer(player);
            
            return Task.CompletedTask;
        });
    
        
        //possibly will be refactored
        dispatcher.Register("make_hiders_phone_sound", async (data, socket) => 
        {
            Console.WriteLine("[make_hiders_phone_sound] Notifying hiders to make sound.");
            var lobbyId = data.GetProperty("lobbyId").GetString();
            await PlayerMessageSender.SendMakeNoise(lobbyId!);
        });
        
        dispatcher.Register("use_ability", async (data, socket) => 
        {
            var username = data.GetProperty("username").GetString();
            var lobbyId = data.GetProperty("lobbyId").GetString();
            var abilityName = data.GetProperty("ability").GetString();
            
            var player = PlayerManager.Instance.GetPlayer(username!);
            var lobby = LobbyManager.Instance.GetLobby(lobbyId!);
            
            var gameSession = lobby!.GetGameSession()!;
            
            var playerGameSession = gameSession.GetPlayerGameSession(player!);
            if (abilityName != null)
            {
                Console.WriteLine(abilityName);
                var ability = playerGameSession?.GetAbilityFromString(abilityName);
                
                
                if (ability != null)
                {
                    Console.WriteLine($"{username} has used an {abilityName} ability!. ");
                    _= playerGameSession?.UseAbility(ability)!;
                }
                else
                {
                    Console.WriteLine("NULL ABILITY ERROR!");
                }
            }
        });
    }
}