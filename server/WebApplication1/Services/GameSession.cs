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

    // Add ping state tracking
    private bool _isPinging = false;
    private Player? _currentPingingPlayer = null;
    private DateTime _pingStartTime;
    private readonly TimeSpan _pingDuration = TimeSpan.FromSeconds(5);
    private readonly TimeSpan _pingCooldown = TimeSpan.FromSeconds(10);

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

    public async Task<bool> RequestPing(Player requestingPlayer)
    {
        // Check if a ping is already active
        if (_isPinging)
        {
            // Ping already active, reject the request
            await GameMessageSender.SendPingRejected(requestingPlayer);
            return false;
        }
        
        // Set ping state
        _isPinging = true;
        _currentPingingPlayer = requestingPlayer;
        _pingStartTime = DateTime.UtcNow;
        
        // Get hiders' locations
        var hiders = _lobby.GetHidersList();
        var staleThreshold = TimeSpan.FromSeconds(6);
        
        await GameMessageSender.RequestHidersLocation(_lobby);
        
        var timeout = TimeSpan.FromSeconds(10);
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
        
        // Send ping activated message to all seekers
        await GameMessageSender.SendPingActivated(_lobby, freshHiders.ToList(), requestingPlayer.Name);
        
        // Start ping timer
        _ = StartPingTimer();
        
        return true;
    }
    
    private async Task StartPingTimer()
    {
        // Wait for ping duration
        await Task.Delay(_pingDuration);
        
        // End ping active state
        _isPinging = false;
        
        // Send ping ended message to all seekers
        await GameMessageSender.SendPingEnded(_lobby, _currentPingingPlayer!.Name);
        
        // Start cooldown timer
        _ = StartCooldownTimer();
    }
    
    private async Task StartCooldownTimer()
    {
        // Wait for cooldown duration
        await Task.Delay(_pingCooldown);
        
        // Send cooldown ended message to all seekers
        await GameMessageSender.SendPingCooldownEnded(_lobby);
    }
    
    public bool IsPingActive()
    {
        return _isPinging;
    }
    
    public Player? GetCurrentPingingPlayer()
    {
        return _currentPingingPlayer;
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

