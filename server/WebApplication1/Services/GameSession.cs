using System.Collections.Concurrent;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services.Messaging;

namespace WebApplication1.Services;

public class GameSession
{
    #region Fields & Constructor

    private readonly Lobby _lobby;
    private readonly ConcurrentDictionary<Player, PlayerGameSession> _playerGameSessions = new();
    private readonly List<Func<Task>> _taskList = [];
    private readonly Random _random = new();

    private TimeSpan _timer = TimeSpan.FromSeconds(25);
    private readonly TimeSpan _tickRate = TimeSpan.FromSeconds(1);
    private readonly TimeSpan _taskInterval = TimeSpan.FromSeconds(15);
    private DateTime _lastTaskSpawnTime;

    public GameSession(Lobby lobby)
    {
        _lobby = lobby;
    }

    #endregion

    #region Game Lifecycle

    public async Task Start()
    {
        _lobby.SetGameSession(this);

        foreach (Player player in _lobby.Players)
        {
            _playerGameSessions[player] = new PlayerGameSession(player, this);
        }

        //await GameMessageSender.SendTimeUpdate(_lobby, _timer);
        _lastTaskSpawnTime = DateTime.UtcNow;

        _ = Task.Run(UpdateLoop);
        _ = Task.Run(MonitorGameTime);
    }

    private async Task MonitorGameTime()
    {
        while (_timer > TimeSpan.Zero)
        {
            await Task.Delay(_tickRate);
            _timer -= _tickRate;
        }

        await EndGame();
    }

    private async Task EndGame()
    {
        await GameMessageSender.SendGameEnded(_lobby);
       // _lobby.ClearGameSession();
    }

    #endregion

    
    #region Timer Adjustment

    public async Task AddTime(TimeSpan duration)
    {
        _timer += duration;
        //await GameMessageSender.SendTimeUpdate(_lobby, _timer);
    }

    public async Task SubtractTime(TimeSpan duration)
    {
        _timer -= duration;
        if (_timer < TimeSpan.Zero)
            _timer = TimeSpan.Zero;

        //await GameMessageSender.SendTimeUpdate(_lobby, _timer);
    }

    #endregion

    #region Ping Mechanic

    public void RequestPing(Player requestingPlayer)
    {
        _ = Task.Run(() => HandlePing(requestingPlayer));
    }

    private async Task HandlePing(Player requestingPlayer)
    {
        var hiders = _lobby.GetHidersList();
        var freshHiders = new HashSet<Player>();

        var timeout = TimeSpan.FromSeconds(10);
        var staleThreshold = TimeSpan.FromSeconds(6);
        var startTime = DateTime.UtcNow;

        await GameMessageSender.RequestHidersLocation(_lobby);

        while (DateTime.UtcNow - startTime < timeout)
        {
            foreach (var hider in hiders.Where(h => h.IsLocationFresh(staleThreshold)))
            {
                freshHiders.Add(hider);
            }

            if (freshHiders.Count == hiders.Count)
                break;

            await Task.Delay(200);
        }

        await GameMessageSender.SendPingToSeekers(_lobby, freshHiders.ToList());
    }

    #endregion
    
    #region Periodic Update & Tasks

    private async Task UpdateLoop()
    {
        while (_timer > TimeSpan.Zero)
        {
            if (DateTime.UtcNow - _lastTaskSpawnTime >= _taskInterval)
            {
                _lastTaskSpawnTime = DateTime.UtcNow;
                await SpawnRandomTask();
            }

            await Task.Delay(_tickRate);
        }
    }

    private async Task SpawnRandomTask()
    {
        if (_taskList.Count == 0)
            return;

        int index = _random.Next(_taskList.Count);
        var selectedTask = _taskList[index];
        await selectedTask();
    }

    public void RegisterTask(Func<Task> task)
    {
        _taskList.Add(task);
    }

    #endregion

    #region Accessors

    public PlayerGameSession? GetPlayerGameSession(Player player) =>
        _playerGameSessions.TryGetValue(player, out var session) ? session : null;

    public Lobby GetLobby() => _lobby;

    #endregion
}
