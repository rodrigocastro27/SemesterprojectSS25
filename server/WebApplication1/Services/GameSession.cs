using System.Collections.Concurrent;
using WebApplication1.Models;
using WebApplication1.Services.Messaging;

namespace WebApplication1.Services;

public class GameSession(Lobby lobby)
{
    private readonly Lobby _lobby = lobby;

    private readonly ConcurrentDictionary<Player, PlayerGameSession> _playerGameSessions = new ConcurrentDictionary<Player, PlayerGameSession>();
    
    private readonly List<Func<Task>> _taskList = [];
    
    private readonly Random _random = new Random();  

    //game session starting logic
    public async Task Start()
    {
        lobby.SetGameSession(this);
        

        foreach (Player p in _lobby.Players)
        {
            _playerGameSessions[p] = new PlayerGameSession(p, this);       //instantiate a player game session per player
        }
        
        
       // await Update();
    }

    private async Task Update()
    {
        while (true)
        {
            await Task.Delay(TimeSpan.FromSeconds(15)); // Every 15s (or whatever interval)
            SpawnRandomTask();
        }
    }


    public async void RequestPing(Player requestingPlayer)
    {
        
        var hiders = _lobby.GetHidersList();
        var staleThreshold = TimeSpan.FromSeconds(6);   //could be altered, maximum time interval allowed from last ping
        
        await GameMessageSender.RequestHidersLocation(_lobby);
        
        var timeout = TimeSpan.FromSeconds(10); //after 10 seconds it quits
        var start = DateTime.UtcNow;

        var freshHiders = new HashSet<Player>();

        while (DateTime.UtcNow - start < timeout)
        {
            foreach (var hider in hiders.Where(hider => hider.IsLocationFresh(staleThreshold)))
            {
                freshHiders.Add(hider);
            }
            
            if (freshHiders.Count == hiders.Count)
                break;

            await Task.Delay(200);
        }

        await GameMessageSender.SendPingToSeekers(_lobby, freshHiders.ToList());
    }
    
    
    public PlayerGameSession GetPlayerGameSession(Player requestingPlayer) => _playerGameSessions[requestingPlayer];
    public Lobby GetLobby() => _lobby;


    private void SpawnRandomTask()
    {
        if (_taskList.Count == 0)
            return;

        int index = _random.Next(_taskList.Count);
        var selectedTask = _taskList[index];
    
        // Fire and forget
        _ = selectedTask();
    }
}