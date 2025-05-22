using System.Collections.Concurrent;
using System.Net.WebSockets;
using WebApplication1.Models;
using System.Data.SQLite;
using WebApplication1.Data;
using WebApplication1.Utils;
using WebApplication1.Handlers;
using System.Threading.Tasks;
using WebApplication1.Services.Messaging;

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
        using var conn = SQLiteConnector.GetConnection();

        // Add player to database
        var cmd1 = new SQLiteCommand("INSERT INTO Players (id, username) VALUES (@id, @username);", conn);
        cmd1.Parameters.AddWithValue("@id", deviceId);
        cmd1.Parameters.AddWithValue("@username", username);
        cmd1.ExecuteNonQuery();

        // Insert into the Manager player list
        var player = new Player(username, deviceId, socket);
        AddPlayer(username, player);
        return player;
    }

    public void RemovePlayer(string username)
    {
        // Delete from database
        using var conn = SQLiteConnector.GetConnection();
        var cmd = new SQLiteCommand("DELETE FROM Players WHERE username = @username;", conn);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.ExecuteNonQuery();

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
        using var conn = SQLiteConnector.GetConnection();

        string? lobbyId = null;

        var cmd = new SQLiteCommand("SELECT Lobby FROM LobbyPlayers WHERE Player = @username;", conn);
        cmd.Parameters.AddWithValue("@username", player.Name);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            lobbyId = reader.IsDBNull(0) ? null : reader.GetString(0);
        }
        return lobbyId!;
    }

    public string AddPlayerToLobby(Player player, Lobby lobby, string nickname, bool isHost)
    {
        Console.WriteLine($"\nAttempting to add player {player.Name} into lobby {lobby.Id}.");

        using var conn = SQLiteConnector.GetConnection();

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
                var cmd = new SQLiteCommand("UPDATE LobbyPlayers SET Nickname = @nickname WHERE Player = @username;", conn);
                cmd.Parameters.AddWithValue("@nickname", nickname);
                cmd.Parameters.AddWithValue("@username", player.Name);
                cmd.ExecuteNonQuery();
            }
            else    // Or another one
            {
                Console.WriteLine("Player is trying to join a DIFFERENT lobby it is currently in.");
                playerInLobby = null!;
            }
        }
        else  // Add the player to the given lobby
        {
            Console.WriteLine($"The player {player.Name} hasn't joined any lobby yet. Adding player in the lobby in the database.");
            // Add player into database
            var cmd = new SQLiteCommand("INSERT INTO LobbyPlayers (Lobby, Player, Nickname, IsHost, Role) VALUES(@lobbyId, @username, @nickname, @isHost, @role);", conn);
            cmd.Parameters.AddWithValue("@lobbyId", lobby.Id);
            cmd.Parameters.AddWithValue("@username", player.Name);
            cmd.Parameters.AddWithValue("@nickname", nickname);
            cmd.Parameters.AddWithValue("@isHost", isHost);
            cmd.Parameters.AddWithValue("@role", player.Role);

            cmd.ExecuteNonQuery();

            lobby.AddPlayer(player);
            playerInLobby = lobby.Id;
        }

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
        if (player._isHost == true)
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
                var cmd1 = new SQLiteCommand("UPDATE LobbyPlayers SET IsHost = 1 WHERE Player = @username;", conn);
                cmd1.Parameters.AddWithValue("@username", newHost.Name);  // username is the player's identifier
                cmd1.ExecuteNonQuery();

                Console.WriteLine($"Notifying all players in lobby {lobby.Id} that the new host is player {player.Name}.");
                await LobbyMessageSender.BroadcastNewHost(lobby, newHost);
            }
            else
            {
                Console.WriteLine($"The lobby is empty. Proceeding to delete lobby from the database.");
                // Delete from database
                var cmd3 = new SQLiteCommand("DELETE FROM Lobbies WHERE `name` = @lobbyId;", conn);
                cmd3.Parameters.AddWithValue("@lobbyId", lobby.Id);
                cmd3.ExecuteNonQuery();

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
        var cmd2 = new SQLiteCommand("DELETE FROM LobbyPlayers WHERE Lobby = @lobbyId AND Player = @username;", conn);
        cmd2.Parameters.AddWithValue("@lobbyId", lobby.Id);
        cmd2.Parameters.AddWithValue("@username", player.Name);
        cmd2.ExecuteNonQuery();
    }
}