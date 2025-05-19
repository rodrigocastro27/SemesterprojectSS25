
using System.Collections.Concurrent;
using WebApplication1.Models;
using System.Data.SQLite;
using WebApplication1.Data;

namespace WebApplication1.Services;

public class LobbyManager
{
    private readonly ConcurrentDictionary<string, Lobby> _lobbies = new();

    public static LobbyManager Instance { get; } = new LobbyManager();

    private LobbyManager() { }

    public void AddLobby(string name, Lobby lobby)
    {
        _lobbies[name] = lobby;
    }

    public Lobby? GetLobby(string lobbyId)
    {
        _lobbies.TryGetValue(lobbyId, out var lobby);
        return lobby;
    }

    public Lobby CreateLobby(string lobbyId)
    {
        using var conn = SQLiteConnector.GetConnection();

        // See if lobby exists
        if (GetLobby(lobbyId) != null) return null!;

        // If it doesn't insert it into the database
        var cmd1 = new SQLiteCommand("INSERT INTO Lobbies (`name`) VALUES (@name);", conn);
        cmd1.Parameters.AddWithValue("@name", lobbyId);
        cmd1.ExecuteNonQuery();

        // Update the manager's data
        var lobby = new Lobby(lobbyId);
        AddLobby(lobbyId, lobby);

        return lobby;
    }

}