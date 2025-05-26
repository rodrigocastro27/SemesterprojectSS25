using System.Collections.Concurrent;
using System.Net.WebSockets;
using WebApplication1.Models;
using System.Data.SQLite;
using WebApplication1.Data;
using WebApplication1.Database;
using WebApplication1.Services.Messaging;
using System.Threading.Tasks;

namespace WebApplication1.Services;

public class PlayerManager
{
    private readonly ConcurrentDictionary<string, Player> _players = new();

    // Singleton instance
    public static PlayerManager Instance { get; } = new PlayerManager();

    private PlayerManager() { } // prevent external instantiation

    public void AddPlayer(string username, Player player)
    {
        _players[username] = player;
    }

    public Player CreatePlayer(string deviceId, string username, WebSocket socket)
    {
        Console.WriteLine("Proceeding to add player in the database.");

        // Add player to database
        DatabaseHandler.Instance.InsertIntoPlayers(deviceId, username);

        // Insert into the Manager player list
        var player = new Player(username, "none", deviceId, socket);
        AddPlayer(username, player);
        return player;
    }

    public void RemovePlayer(string username)
    {
        // Delete from database
        DatabaseHandler.Instance.DeleteFromPlayers(username);

        // Remove from player list
        _players.TryRemove(username, out _);
    }

    public void UpdatePlayerSocket(string id, WebSocket newSocket)
    {
        if (_players.TryGetValue(id, out var player))
        {
            player.Socket = newSocket;
        }
    }

    public Player? GetPlayer(string id)
    {
        _players.TryGetValue(id, out var player);
        return player;
    }

    public Player? GetPlayerByName(string username)
    {
        return _players.Values.FirstOrDefault(player => player.Name == username, null!);
    }

    public IEnumerable<Player> GetAllPlayers() => _players.Values;

    public void PrintPlayers()
    {
        Console.WriteLine("\n\n");
        foreach (var player in _players.Values)
        {
            Console.WriteLine($"Player Name: {player.Name}");
        }
        Console.WriteLine("\n\n");
    }

    public string IsPlayerInLobby(Player player) {

        string lobbyId = DatabaseHandler.Instance.SelectLobbyFromLobbyPlayers(player.Name);

        return lobbyId;
    }

    public string AddPlayerToLobby(Player player, Lobby lobby, string nickname, bool isHost, string role)
    {
        Console.WriteLine($"\nAttempting to add player {player.Name} into lobby {lobby.Id}.");

        string playerInLobby = IsPlayerInLobby(player);

        // If the player is already in a lobby
        if (playerInLobby != null)
        {
            Console.WriteLine($"Player {player.Name} is already in lobby {playerInLobby}.");
            // Check if trying to access the same one
            if (playerInLobby == lobby.Id)
            {
                Console.WriteLine("Player is trying to join the SAME lobby it is currently in. Updating player's nickname.");
                // Update the nickname if it has been changed
                DatabaseHandler.Instance.UpdetLobbyPlayersNickname(player.Name, nickname, role);
            }
            else    // Or another one
            {
                Console.WriteLine("Player is trying to join a DIFFERENT lobby it is currently in. Changing player from lobbies.");
                // Update the lobby in the database
                DatabaseHandler.Instance.UpdateLobbyPlayersLobby(player.Name, lobby.Id, role);
                // Update the manager's lists
                Lobby? oldLobby = LobbyManager.Instance.GetLobby(playerInLobby);
                oldLobby!.RemovePlayer(player);
                lobby.AddPlayer(player);
            }
        }
        else  // Add the player to the given lobby
        {
            Console.WriteLine($"The player {player.Name} hasn't joined any lobby yet. Adding player in the lobby in the database.");
            // Add player into database
            DatabaseHandler.Instance.InsertIntoLobbyPlayers(lobby.Id, player.Name, nickname, isHost, role);

            lobby.AddPlayer(player);
            playerInLobby = lobby.Id;
        }

        player.SetRole(role == "hider" ? Role.hider : Role.seeker);

        Console.WriteLine($"Finished attempt to add player {player.Name} in lobby {lobby.Id}.");

        return playerInLobby;
    }

    public async void RemovePlayerFromLobby(Player player, Lobby lobby)
    {
        Console.WriteLine($"\nProceding to remove player {player.Name} from lobby {lobby.Id} in the database.");

        using var conn = SQLiteConnector.GetConnection();

        Player newHost = null!;

        lobby.RemovePlayer(player);

        // See if player is host and act accordingly
        if (player.IsHost == true)
        {
            Console.WriteLine($"Player {player.Name} is the host of the lobby. Proceeding to change it.");
            // Set new host if there is someone still left in the lobby
            newHost = lobby.GetRandomPlayer();

            if (newHost != null)
            {
                Console.WriteLine($"The lobby is not empty. Proceeding to make player {newHost.Name} the new host of lobby {lobby.Id} in the database.");

                // Change manager
                newHost.SetHost(true);

                // Change in database
                DatabaseHandler.Instance.UpdateLobbyPlayersHost(newHost.Name);

                Console.WriteLine($"Notifying the new host {player.Name} in lobby {lobby.Id} that it is the new host.");
                await LobbyMessageSender.SetNewHost(newHost);
            }
            else
            {
                Console.WriteLine($"The lobby is empty. Proceeding to delete lobby from the database.");
                // Delete from database
                DatabaseHandler.Instance.DeleteFromLobbies(lobby.Id);

                // Delete from manager
                Lobby res = LobbyManager.Instance.DeleteLobby(lobby);
                if (res == null)
                {
                    Console.WriteLine("Lobby did not exist before removing or error removing.");
                }
            }
        }

        Console.WriteLine($"The player {player.Name} is not the host of lobby {lobby.Id}. Proceeding to delete the player from the lobby in database.");

        // Delete player from lobby in database
        DatabaseHandler.Instance.DeleteFromLobbyPlayersPlayer(lobby.Id, player.Name);
    }
}