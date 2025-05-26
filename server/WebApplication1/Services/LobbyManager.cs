
using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;
using WebApplication1.Models;
using WebApplication1.Database;

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

        // See if lobby exists
        if (GetLobby(lobbyId) != null) return null!;

        // If it doesn't insert it into the database
        DatabaseHandler.Instance.InsertIntoLobbies(lobbyId);

        // Update the manager's data
        var lobby = new Lobby(lobbyId);
        AddLobby(lobbyId, lobby);

        return lobby;
    }

    public bool IsLobbyEmpty(string lobbyId)
    {
        Lobby? lobby = GetLobby(lobbyId);
        return lobby!.Players.Count == 0;
    }

    public bool IsHost(string username)
    {
        return DatabaseHandler.Instance.SelectIsHostFromLobbyPlayers(username);
    }

    public Lobby DeleteLobby(Lobby lobby)
    {
        if (GetLobby(lobby.Id) == null) return null!;

        Console.WriteLine($"Proceeding to delete lobby {lobby.Id} from database.");

        // Delete from database
        DatabaseHandler.Instance.DeleteFromLobbies(lobby.Id);

        // Delete from manager
        _lobbies.Remove(lobby.Id, out var result);

        return result!;
    }
    
    public void DeleteEmptyLobbies()
    {
        foreach (var elem in _lobbies)
        {
            var lobby = elem.Value;
            if (IsLobbyEmpty(lobby.Id))
            {
                Console.WriteLine($"Lobby {elem.Key} has no players.");
                DeleteLobby(lobby);
            }
        }
    }
}