using WebApplication1.Models;
using WebApplication1.Services.Messaging;

namespace WebApplication1.Services;

public class PlayerGameSession(Player player, GameSession gameSession)
{
    private Player _player = player;
    private GameSession _gameSession = gameSession;

    public Dictionary<string, System.Text.Json.JsonElement> _taskUpdates = new();

    public void MarkUpdateReceived(string taskName, System.Text.Json.JsonElement info)
    {
        _taskUpdates[taskName] = info;
    }

    public bool HasSentUpdate(string taskName)
    {
        if (_taskUpdates == null)
            return false;

        return _taskUpdates.TryGetValue(taskName, out var value) && value.ValueKind != System.Text.Json.JsonValueKind.Undefined;;
    }

    public System.Text.Json.JsonElement GetInfoFrom(string taskName)
    {
        _taskUpdates.TryGetValue(taskName, out var value);
        return value!;
    }
    /*      logic for indevidually pinging without global synchronization
    private Player _player = player;
    private readonly GameSession _gameSession = gameSession;
    private int _hidersCount;
    private int _totalHindersCount;

    public bool isWaiting = false;
    public async Task Ping()
    {
        isWaiting = true;
        _hidersCount = 0;
        _totalHindersCount = _gameSession.GetLobby().Players.Count(p => p.GetRole_s() == "hider");
        await GameSessionMessageSender.RequestPlayersLocation(_gameSession.GetLobby());
    }

    public async Task RecievedPing()
    {
        _hidersCount++;
        if (_hidersCount == _totalHindersCount)
        {
            isWaiting = false;
            await GameSessionMessageSender.RequestPlayersLocation(_gameSession.GetLobby());
            _totalHindersCount = 0;
            _hidersCount = 0;
        }
        
    }*/
}