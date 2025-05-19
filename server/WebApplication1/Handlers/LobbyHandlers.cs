using System.Net.WebSockets;
using System.Text.Json;
using WebApplication1.Services;
using WebApplication1.Utils;

namespace WebApplication1.Handlers;

public static class LobbyHandlers
{
    public static void Register(WebSocketActionDispatcher dispatcher)
    {   
        dispatcher.Register("create_lobby", async (data, socket) =>
        {
            Console.WriteLine("\nPROCESSING CREATING LOBBY...\n");

            if (data.TryGetProperty("username", out var nameElem) &&
                data.TryGetProperty("lobbyId", out var lobbyIdElem))
            {
                var username = nameElem.GetString();
                var lobbyId = lobbyIdElem.GetString();

                Console.WriteLine("\nLobby: $lobbyId\n");

                var player = PlayerManager.Instance.GetPlayer(username!);
                var lobby = LobbyManager.Instance.CreateLobby(lobbyId!);

                if (lobby == null)
                {
                    Console.WriteLine("\nFAILED TO JOIN LOBBY\n");
                    await MessageSender.SendToPlayerAsync(player!, "failed_lobby", new { lobbyId = lobbyId });
                }
                else
                {
                    // Modify the Managers
                    player!.SetHost(true);

                    // Add player to lobby in the database
                    string lobbyAdded = PlayerManager.Instance.AddPlayerToLobby(player, lobby, username!, true);
                    if (lobbyAdded == null)
                    {
                        await MessageSender.SendToPlayerAsync(player, "player_already_in_lobby", new
                        {
                            username = player.Name,
                            lobbyId = lobby.Id,
                        });
                        return;
                    }

                    await MessageSender.SendToPlayerAsync(player, "lobby_created", new
                    {
                        lobbyId = lobbyId,
                        player = new
                        {
                            name = player.Name,
                            role = player.Role
                        }
                    });
                }
            }
        });


        dispatcher.Register("join_lobby", async (data, socket) =>
        {   
            // Read data in message
            var username = data.GetProperty("username").GetString();
            var lobbyId = data.GetProperty("lobbyId").GetString();
            var nickname = data.GetProperty("nickname").GetString();

            // Check if player is already in the list of players
            var player = PlayerManager.Instance.GetPlayer(username!);
            var lobby = LobbyManager.Instance.GetLobby(lobbyId!);

            // Handle if lobby does not exist       
            if (lobby == null)
            {
                await socket.SendAsync(JsonSerializer.SerializeToUtf8Bytes(new {
                    action = "failed_lobby",
                    lobbyId
                }), WebSocketMessageType.Text, true, CancellationToken.None);    
                return;
            }
            
            player!.SetHost(false);  // Don't add the player as host

            // Add player to lobby in the database
            string lobbyAdded = PlayerManager.Instance.AddPlayerToLobby(player, lobby, nickname!, false);
            if (lobbyAdded == null)
            {
                await MessageSender.SendToPlayerAsync(player, "player_already_in_lobby", new
                {
                    username = player.Name,
                    lobbyId = lobby.Id,
                });
                return;
            }
            
            //confirmation message 
            await MessageSender.SendToPlayerAsync(player, "lobby_joined", new
            {
                lobbyId = lobbyId,
                host = false,
                player = new
                {
                    name = player.Name,
                    role = player.Role
                }
            });
            await MessageSender.BroadcastLobbyAsync(lobby, "new_player_joined", new
            {
                player = new {
                    name = player.Name,
                    role = player.Role
                }
            });
        });

        
        dispatcher.Register("exit_lobby", async (data, socket) =>
        {
            var username = data.GetProperty("username").GetString();
            var lobbyId = data.GetProperty("lobbyId").GetString();
            
            var lobby = LobbyManager.Instance.GetLobby(lobbyId!);
            var player = PlayerManager.Instance.GetPlayer(username!);

            if (lobby == null||player == null)
            {
                //implement with message sender
                await socket.SendAsync(JsonSerializer.SerializeToUtf8Bytes(new {
                    action = "failed",
                    lobbyId
                }), WebSocketMessageType.Text, true, CancellationToken.None);    
                return;
            }
            
            // Update manager
            lobby.RemovePlayer(player);

            // Eliminate from the lobby
            PlayerManager.Instance.RemovePlayerFromLobby(player, lobby);
            
            //implement with message sender 
            await socket.SendAsync(JsonSerializer.SerializeToUtf8Bytes(new
            {
                action = "player_removed",
                lobbyId
            }), WebSocketMessageType.Text, true, CancellationToken.None);
            
        });
        
        
        dispatcher.Register("start_game", async (data, socket) =>
        {
            var lobbyId = data.GetProperty("lobbyId").GetString();
            
            var lobby = LobbyManager.Instance.GetLobby(lobbyId!);
            
            await MessageSender.BroadcastLobbyAsync(lobby!, "game_started", new
            {
                action = "started",
            });
        });
    }
}
