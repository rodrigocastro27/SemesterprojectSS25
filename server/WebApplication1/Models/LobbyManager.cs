namespace WebApplication1.Models;

public class LobbyManager
{
    private readonly Dictionary<string, Lobby> _lobbies = new();

    public Lobby GetOrCreateLobby(string lobbyId)
    {
        if (!_lobbies.ContainsKey(lobbyId))
        {
            _lobbies[lobbyId] = new Lobby(lobbyId);
        }

        return _lobbies[lobbyId];
    }

    public Lobby CreateLobby(string lobbyId, string userId)
    {
        if (!_lobbies.ContainsKey(lobbyId))
        {
            _lobbies[lobbyId] = new Lobby(lobbyId);
        }
        return _lobbies[lobbyId];
    }
    
    
    public bool CanJoinLobby(string lobbyId, string playerId)
    {
        foreach (var lobby in _lobbies)
        {
            
        }
        return false;
    }
}