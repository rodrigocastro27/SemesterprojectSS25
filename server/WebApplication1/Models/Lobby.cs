using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WebApplication1.Services;

namespace WebApplication1.Models;

public class Lobby
{
    public string Id { get; }
    public List<Player> Players { get; } = new();
    private bool _timerRunning = false;

    private GameSession _session;

    public Lobby(string id)
    {
        Id = id;
    }

    public void AddPlayer(Player player)
    {
        Players.Add(player);
    }

    public void RemovePlayer(Player player)
    {
        Players.Remove(player);
    }

    public bool HasPlayer(Player player)
    {
        return Players.Any(p => p.Id == player.Id);
    }

    public Player GetRandomPlayer()
    {
        if (Players.Count == 0) return null!;
        return Players[0];  // return first player in list, to select someone
    }
    
    public void SetGameSession(GameSession current)
    {
        _session = current;
    }

    public GameSession GetGameSession() => _session;


    public List<Player> GetHidersList()
    {
        return Players.Where(p => p.GetRole() == Role.hider).ToList();
    }

    public List<Player> GetSeekerList()
    {
        return Players.Where(p => p.GetRole() == Role.seeker).ToList();
    }

}