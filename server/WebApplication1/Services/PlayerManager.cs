using System.Collections.Concurrent;
using System.Net.WebSockets;
using WebApplication1.Models;
using System.Data.SQLite;
using WebApplication1.Data;

namespace WebApplication1.Services;

public class PlayerManager
{
    private readonly ConcurrentDictionary<string, Player> _players = new();

    // Singleton instance
    public static PlayerManager Instance { get; } = new PlayerManager();

    private PlayerManager() { } // prevent external instantiation

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
        _players[username] = player;
        return player;
    }

    public void Remove(string id)
    {
        _players.TryRemove(id, out _);
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
        Console.WriteLine("\n\n\n\n");
        foreach (var player in _players.Values)
        {
            Console.WriteLine($"Player Name: {player.Name}");
        }
    }

    public void AddPlayer(string username, Player player)
    {
        _players[username] = player;
    }
}