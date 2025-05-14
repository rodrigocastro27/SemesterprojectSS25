
using System.Collections.Concurrent;
using WebApplication1.Models;

namespace WebApplication1.Services;

public class LobbyManager
{
    private readonly ConcurrentDictionary<string, Lobby> _lobbies = new();

    public static LobbyManager Instance { get; } = new LobbyManager();

    private LobbyManager() {}

    public Lobby? GetLobby(string lobbyId)
    {
        _lobbies.TryGetValue(lobbyId, out var lobby);
        return lobby;
    }

    public Lobby CreateLobby(string lobbyId)
    {
        var lobby = new Lobby(lobbyId);
        _lobbies[lobbyId] = lobby;
        return lobby;
    }

}