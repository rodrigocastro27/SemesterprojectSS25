using System.Collections.Concurrent;
using System.Net.WebSockets;
using WebApplication1.Models;
using System.Data.SQLite;
using WebApplication1.Data;
using WebApplication1.Utils;
using WebApplication1.Handlers;
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
        using var conn = SQLiteConnector.GetConnection();

        string playerInLobby = IsPlayerInLobby(player);

        Console.WriteLine($"Player {player.Name} is in lobby {playerInLobby}. Trying to join lobby {lobby.Id}.");

        // If the player is already in a lobby
        if (playerInLobby != null)
        {
            // Check if trying to access the same one
            if (playerInLobby == lobby.Id)
            {
                Console.WriteLine("TRYING TO JOIN SAME LOBBY!");
                // Update the nickname if it has been changed
                var cmd = new SQLiteCommand("UPDATE LobbyPlayers SET Nickname = @nickname WHERE Player = @username;", conn);
                cmd.Parameters.AddWithValue("@nickname", nickname);
                cmd.Parameters.AddWithValue("@username", player.Name);
                cmd.ExecuteNonQuery();
            }
            else    // Or another one
            {
                Console.WriteLine("TRYING TO JOIN DIFFERENT LOBBY!");
                playerInLobby = null!;
            }
        }
        else  // Add the player to the given lobby
        {
            Console.WriteLine($"ADDING PLAYER IN LOBBY ${lobby.Id}.");
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

        return playerInLobby;
    }

    public async void RemovePlayerFromLobby(Player player, Lobby lobby)
    {
        Console.WriteLine("\nProceding to remove player from lobby in database.\n");

        using var conn = SQLiteConnector.GetConnection();

        lobby.RemovePlayer(player);

        // See if player is host and act accordingly
        if (player._isHost == true)
        {
            // Set new host if there is someone still left in the lobby
            Player newHost = lobby.GetRandomPlayer();

            if (newHost != null)
            {
                Console.WriteLine("\n\t\tSUBCASE: There's someone left in the lobby\n");
                // TODO: test if it updates properly (with someone else)

                // Change manager
                newHost.SetHost(true);
                await MessageSender.BroadcastLobbyAsync(lobby, "new_host", new
                {
                    player = player.Name,
                }); // Notify the handler to send a message to the UI

                // Change in database
                var cmd1 = new SQLiteCommand("UPDATE LobbyPlayers SET IsHost = 1 WHERE Player = @username;", conn);
                cmd1.Parameters.AddWithValue("@username", newHost.Name);  // username is the player's identifier
                cmd1.ExecuteNonQuery();
            }
            else
            {
                // Delete from database
                var cmd3 = new SQLiteCommand("DELETE FROM Lobbies WHERE `name` = @lobbyId;", conn);
                cmd3.Parameters.AddWithValue("@lobbyId", lobby.Id);
                cmd3.ExecuteNonQuery();

                // Delete from manager
                Lobby res = LobbyManager.Instance.DeleteLobby(lobby);
                if (res == null)
                {
                    Console.WriteLine("\nLobby did not exist before removing or error removing.\n");
                }
            }
        }

        // Delete player from lobby in database
        var cmd2 = new SQLiteCommand("DELETE FROM LobbyPlayers WHERE Lobby = @lobbyId AND Player = @username;", conn);
        cmd2.Parameters.AddWithValue("@lobbyId", lobby.Id);
        cmd2.Parameters.AddWithValue("@username", player.Name);
        cmd2.ExecuteNonQuery();
    }
}