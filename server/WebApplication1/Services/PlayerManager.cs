using System.Collections.Concurrent;
using System.Net.WebSockets;
using WebApplication1.Models;

namespace WebApplication1.Services;

public class PlayerManager
{
    private readonly ConcurrentDictionary<int, Player> _players = new();

    // Singleton instance
    public static PlayerManager Instance { get; } = new PlayerManager();

    private PlayerManager() {} // prevent external instantiation

    public void Register(Player player)
    {
        _players[player.Id] = player;
    }

    public void Remove(int id)
    {
        _players.TryRemove(id, out _);
    }

    public void UpdatePlayerSocket(int id, WebSocket newSocket)
    {
        if (_players.TryGetValue(id, out var player))
        {
            player.Socket = newSocket;
        }
    }

    public Player? GetPlayer(int id)
    {
        _players.TryGetValue(id, out var player);
        return player;
    }

    public Player CreatePlayer(int id, string name, WebSocket socket)
    {
        var player = new Player(name, id, socket);
        _players[id] = player;
        return player;
    }
    
  
    
    public IEnumerable<Player> GetAllPlayers() => _players.Values;
}