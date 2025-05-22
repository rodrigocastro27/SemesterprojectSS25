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
        dispatcher.Register("create_lobby", async (data, socket) =>
        {
            if (data.TryGetProperty("username", out var nameElem) &&
                data.TryGetProperty("lobbyId", out var lobbyIdElem))
            {               
                var username = nameElem.GetString();
                var lobbyId = lobbyIdElem.GetString();

                Console.WriteLine($"\n[create_lobby] Atempting to create lobby {lobbyId}.");

                var player = PlayerManager.Instance.GetPlayer(username!);
                var lobby = LobbyManager.Instance.CreateLobby(lobbyId!);

                if (lobby == null)
                {
                    Console.WriteLine($"Failed to create lobby {lobbyId} because it doesn't exist. Notifying client...");
                    await LobbyMessageSender.ErrorMessageAsync(lobbyId!, player!, 1);
                    //await MessageSender.SendToPlayerAsync(player!, "failed_lobby", new { lobbyId = lobbyId });
                }
                else
                {
                    Console.WriteLine($"Successfully created lobby {lobbyId}.");

                    Console.WriteLine($"Setting player {player!.Name} as host.");
                    // Modify the Managers
                    player!.SetHost(true);

                    // Add player to lobby in the database
                    string lobbyAdded = PlayerManager.Instance.AddPlayerToLobby(player, lobby, username!, true);
                    if (lobbyAdded == null)
                    {
                        Console.WriteLine($"Notifying client that player cannot join the lobby.");
                        await LobbyMessageSender.ErrorMessageAsync(lobby.Id, player, 2);
                        return;
                    }

                    Console.WriteLine($"Notifying client that player {player.Name} successfully joined lobby {lobby.Id}.");
                    await LobbyMessageSender.CreatedLobbyAsync(lobby, player);
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

            Console.WriteLine($"\n[join_lobby] Player {player} attempting to join lobby {lobbyId}.");

            // Handle if lobby does not exist       
            if (lobby == null)
            {
                Console.WriteLine($"Lobby {lobby!.Id} does not exist. Notifying the client.");
                await LobbyMessageSender.ErrorMessageAsync(lobbyId!, player!, 1);
                return;
            }
            
            // Add player to lobby in the database as host in case it is empty
            bool isHost = lobby.GetRandomPlayer() == null ? true : false;
            player!.SetHost(isHost);
            if (isHost) Console.WriteLine($"Lobby {lobby.Id} is empty so adding player {player.Name} as host.");
            else Console.WriteLine($"Lobby {lobby.Id} is not empty so adding player {player.Id} NOT as host.");
            
            string lobbyAdded = PlayerManager.Instance.AddPlayerToLobby(player, lobby, nickname!, isHost);
            if (lobbyAdded == null)
            {
                Console.WriteLine($"Notifying client that player cannot join the lobby.");
                await LobbyMessageSender.ErrorMessageAsync(lobbyId!, player, 2);
                return;
            }

            //confirmation message
            Console.WriteLine($"Notifying player {player.Name} that he successfully joined lobby {lobby.Id}.");
            await LobbyMessageSender.JoinedAsync(lobby, player, isHost);
            Console.WriteLine($"Notifying all players in lobby {lobby.Id} that player {player.Id} joined the lobby.");
            await LobbyMessageSender.BroadcastPlayerList(lobby);
        });

        
        dispatcher.Register("exit_lobby", async (data, socket) =>
        {
            var username = data.GetProperty("username").GetString();
            var lobbyId = data.GetProperty("lobbyId").GetString();

            var lobby = LobbyManager.Instance.GetLobby(lobbyId!);
            var player = PlayerManager.Instance.GetPlayer(username!);

            Console.WriteLine($"\n[exit_lobby] Start procedure to leave lobby {lobbyId}.");

            if (lobby == null)
            {
                Console.WriteLine($"Lobby {lobbyId} does not exist. Notifying player.");

                //implement with message sender
                await LobbyMessageSender.ErrorMessageAsync(lobbyId!, player!, 1);
                return;
            }

            // Update manager
            lobby.RemovePlayer(player!);

            // Eliminate from the lobby in the database
            PlayerManager.Instance.RemovePlayerFromLobby(player!, lobby);

            Console.WriteLine($"Notifyin client that the player was succesfully removed from the lobby.");
            await LobbyMessageSender.LeaveAsync(lobby, player!);
            Console.WriteLine($"Notifying all the players in the lobby that player {player!.Name} left the lobby.");
            await LobbyMessageSender.BroadcastPlayerList(lobby);            
        });


        dispatcher.Register("delete_lobby", async (data, socket) =>
        {
            var lobbyId = data.GetProperty("lobbyId").GetString();
            var lobby = LobbyManager.Instance.GetLobby(lobbyId!);

            Console.WriteLine($"[delete_lobby] Proceeding to delete lobby {lobbyId}.");

            Console.WriteLine("Notifying players still in the lobby that it has been deleted.");
            await LobbyMessageSender.DeletedLobby(lobby!);

            LobbyManager.Instance.DeleteLobby(lobby!);
        });


        dispatcher.Register("start_game", async (data, socket) =>
        {
            var lobbyId = data.GetProperty("lobbyId").GetString();

            Console.WriteLine($"\n[start_game] Proceeding to start game for lobby {lobbyId}.");
            
            var lobby = LobbyManager.Instance.GetLobby(lobbyId!);

            Console.WriteLine("Notifying all players in the lobby that the game is starting.");
            await GameMessageSender.SendGameStarted(lobby!, "started");
        });
    }
}
