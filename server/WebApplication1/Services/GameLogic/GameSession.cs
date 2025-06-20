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
    private readonly List<GameTask> _taskList = [];
    private readonly Random _random = new();

    private Tuple<Double,Double>? _centerMap;

    private TimeSpan _timer = TimeSpan.FromMinutes(10);
    private readonly TimeSpan _tickRate = TimeSpan.FromSeconds(1);
    private readonly TimeSpan _taskInterval = TimeSpan.FromSeconds(15);
    private DateTime _lastTaskSpawnTime;

    public GameSession(Lobby lobby)
    {
        _lobby = lobby;
        RegisterTask(new ClickingRaceTask());
        // Register more tasks here
        // ...
    }

    public void SetMapCenter(double centerLat, double centerLon)
    {
        _centerMap = Tuple.Create(centerLat, centerLon);
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

        await GameMessageSender.SendTimeUpdate(_lobby, _timer);
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
        _lobby.ClearGameSession();
    }

    #endregion

    
    #region Timer Adjustment

    public async Task AddTime(TimeSpan duration)
    {
        _timer += duration;
        await GameMessageSender.SendTimeUpdate(_lobby, _timer);
    }

    public async Task SubtractTime(TimeSpan duration)
    {
        _timer -= duration;
        if (_timer < TimeSpan.Zero)
            _timer = TimeSpan.Zero;

        await GameMessageSender.SendTimeUpdate(_lobby, _timer);
    }

    #endregion

    #region Ping Mechanic

    public void RequestPing(Player requestingPlayer)
    {
        _ = Task.Run(InternalHandlePing);
    }
    
    private TaskCompletionSource<bool>? _pingCompletedSource;

    public Task HandlePing()
    {
        _pingCompletedSource = new TaskCompletionSource<bool>();
        _ = InternalHandlePing(); // fire-and-forget async logic
        return _pingCompletedSource.Task;
    }
    
    private async Task InternalHandlePing()
    {
        var hiders = _lobby.GetHidersList();

        var visibleHiders = hiders
            .Where(h =>
                _playerGameSessions.TryGetValue(h, out var session) &&
                session.CheckVisibility())
            .ToList();

        
        // Request locations from visible hiders only
        await GameMessageSender.RequestPlayersLocation(visibleHiders);

        var freshHiders = new HashSet<Player>();
        var timeout = TimeSpan.FromSeconds(10);
        var staleThreshold = TimeSpan.FromSeconds(6);
        var startTime = DateTime.UtcNow;

        while (DateTime.UtcNow - startTime < timeout)
        {
            foreach (var hider in visibleHiders.Where(h => h.IsLocationFresh(staleThreshold)))
            {
                freshHiders.Add(hider);
            }

            if (freshHiders.Count == visibleHiders.Count)
                break;

            await Task.Delay(200);
        }

        await GameMessageSender.SendPingToSeekers(_lobby, freshHiders.ToList());

        _pingCompletedSource?.TrySetResult(true);
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
                // SpawnRandomTask();
            }

            await Task.Delay(_tickRate);
        }
    }

    private GameTask? SpawnRandomTask()
    {
        if (_taskList.Count == 0)
            return null;

        int index = _random.Next(_taskList.Count);
        var selectedTask = _taskList[index];
        return selectedTask;
    }

    public void RegisterTask(GameTask task)
    {
        _taskList.Add(task);
    }

    public async Task StartTask(Lobby lobby)
    {
        Console.WriteLine("\n[task] Selecting task to play.");

        GameTask? selectedTask = SpawnRandomTask();
        if (selectedTask != null)
        {
            Console.WriteLine($"[task] Executing task {selectedTask.GetName()}");
            await GameMessageSender.BroadcastTask(lobby, selectedTask);

            Task waitingTask = WaitForAllPlayersUpdateAsync(lobby, selectedTask, TimeSpan.FromSeconds(25));
            Task executionTask = selectedTask.ExecuteAsync(lobby);

            await Task.WhenAny(waitingTask, executionTask); // for both tasks to run in parallel!!
        }
        else
        {
            Console.WriteLine("[task] Could not find any task to play.");
        }
    }

    public GameTask? GetTask(string taskName)
    {
        foreach (var task in _taskList)
        {
            if (task.Name == taskName) return task;
        }
        return null;
    }

    public async Task WaitForAllPlayersUpdateAsync(Lobby lobby, GameTask task, TimeSpan timeout)
    {
        Console.WriteLine("Starting to wait for player responses from task...");

        var startTime = DateTime.UtcNow;

        var playerSessions = _playerGameSessions.Values.ToList();  // all PlayerSessions in this game
            
        
        var respondedSessions = new HashSet<PlayerGameSession>();

        while (DateTime.UtcNow - startTime < timeout)
        {
            foreach (var session in playerSessions)
            {
                if (session.HasSentUpdate(task.GetName()))
                {
                    respondedSessions.Add(session);
                }
            }

            if (respondedSessions.Count == playerSessions.Count)
                break;  // all players responded

            await Task.Delay(200);  // small delay before checking again
        }

        Console.WriteLine("Finished waiting for players...");


        await task.EndTask(lobby, respondedSessions);
    }

    #endregion

    #region Accessors

    public PlayerGameSession? GetPlayerGameSession(Player player) =>
        _playerGameSessions.TryGetValue(player, out var session) ? session : null;

    public Lobby GetLobby() => _lobby;

    #endregion


    #region Elimination

    public void EliminatePlayer(Player player)
    {
        if(_playerGameSessions.TryGetValue(player, out var session))
        {
            session.Eliminate();
            _ = GameMessageSender.BroadcastEliminatedPlayer(_lobby, player);
        }
    }

    #endregion
}
