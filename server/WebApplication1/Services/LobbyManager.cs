
using System.Collections.Concurrent;
using WebApplication1.Models;
using System.Data.SQLite;
using WebApplication1.Data;
using System.Threading.Tasks;
using WebApplication1.Utils;

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
        Console.WriteLine($"Creating lobby {lobbyId} in database.");
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

    public Lobby DeleteLobby(Lobby lobby)
    {
        if (GetLobby(lobby.Id) == null) return null!;

        Console.WriteLine($"Proceeding to delete lobby {lobby.Id} from database.");

        // Delete from database
        using var conn = SQLiteConnector.GetConnection();
        var cmd = new SQLiteCommand("DELETE FROM Lobbies WHERE `name` = @lobbyId;", conn);
        cmd.Parameters.AddWithValue("@lobbyId", lobby.Id);
        cmd.ExecuteNonQuery();

        // Delete from manager
        _lobbies.Remove(lobby.Id, out var result);

        return result!;
    }

    public void DeleteEmptyLobbies()
    {
        foreach (var elem in _lobbies)
        {
            var lobby = elem.Value;
            if (lobby.Players.Count == 0)
            {
                Console.WriteLine($"Lobby {elem.Key} has no players.");
                DeleteLobby(lobby);
            }
        }
    }
}