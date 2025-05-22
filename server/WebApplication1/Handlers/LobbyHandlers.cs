using System.Net.WebSockets;
using System.Text.Json;
using WebApplication1.Services;
using WebApplication1.Services.Messaging;
using WebApplication1.Utils;

namespace WebApplication1.Handlers;

public static class LobbyHandlers
{
    public static void Register(WebSocketActionDispatcher dispatcher)
    {
        dispatcher.Register("join_lobby", async (data, socket) =>
        {
            var name = data.GetProperty("name").GetString();
            var playerId = data.GetProperty("id").GetInt32();
            var lobbyId = data.GetProperty("lobbyId").GetString();

                
            var player = PlayerManager.Instance.GetPlayer(playerId) 
                         ?? PlayerManager.Instance.CreatePlayer(playerId, name, socket);
         
            var lobby = LobbyManager.Instance.GetLobby(lobbyId);

            
            
            if (lobby == null)
            {
                
                // need to find standard for error messages
                await socket.SendAsync(JsonSerializer.SerializeToUtf8Bytes(new {
                    action = "failed_lobby",
                    lobbyId
                }), WebSocketMessageType.Text, true, CancellationToken.None);    
                return;
            }
            
            lobby.AddPlayer(player);
            player.SetHost(false);



                Console.WriteLine(lobby.Id + ": ");
            
            foreach (var lobbyPlayer in lobby.Players)
            {
                Console.WriteLine(lobbyPlayer.Name);
            }
            
            
            //confirmation message 
            await LobbyMessageSender.JoinedAsync(lobby, player);
            
            //broadcast to other players in the lobby that a new player joined (yes this is an actual human comment, not chatgpt)
            await LobbyMessageSender.BroadcastPlayerJoinedAsync(lobby, player);
        });
        
        
        dispatcher.Register("create_lobby", async (data, socket) =>
        {
            if (data.TryGetProperty("name", out var nameElem) &&
                data.TryGetProperty("id", out var idElem) &&
                data.TryGetProperty("lobbyId", out var lobbyIdElem))
            {
                var name = nameElem.GetString();
                var playerId = idElem.GetInt32();
                var lobbyId = lobbyIdElem.GetString();

                var player = PlayerManager.Instance.GetPlayer(playerId)
                             ?? PlayerManager.Instance.CreatePlayer(playerId, name, socket);

                var lobby = LobbyManager.Instance.CreateLobby(lobbyId);
                lobby.AddPlayer(player);
                player.SetHost(true);

                await LobbyMessageSender.CreateAsync(lobby, player);
            }
        });

        
        dispatcher.Register("exit_lobby", async (data, socket) =>
        {
            var name = data.GetProperty("name").GetString();
            var playerId = data.GetProperty("id").GetInt32();
            var lobbyId = data.GetProperty("lobbyId").GetString();
            
           
            var lobby = LobbyManager.Instance.GetLobby(lobbyId);
            var player = PlayerManager.Instance.GetPlayer(playerId);

            if (lobby == null||player == null)
            {
                await LobbyMessageSender.ErrorMessageAsync(lobby, player);        
                return;
            }
            
            lobby.RemovePlayer(player);
            
           await LobbyMessageSender.LeaveAsync(lobby, player);
            
        });
        
        
        dispatcher.Register("start_game", async (data, socket) =>
        {
            var lobbyId = data.GetProperty("lobbyId").GetString();
            
            var lobby = LobbyManager.Instance.GetLobby(lobbyId);

            await LobbyMessageSender.StartGame(lobby);
            
            
        });
    }
}
