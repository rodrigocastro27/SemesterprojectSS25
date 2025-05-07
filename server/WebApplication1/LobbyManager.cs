using WebApplication1.Models;

namespace WebApplication1;

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
}