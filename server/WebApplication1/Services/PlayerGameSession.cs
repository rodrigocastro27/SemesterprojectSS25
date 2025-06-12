using WebApplication1.Models;
using WebApplication1.Services.Messaging;

namespace WebApplication1.Services;

public class PlayerGameSession(Player player, GameSession gameSession)
{
    private Player _player = player;
    private GameSession _gameSession = gameSession;

    public Dictionary<string, System.Text.Json.JsonElement> _taskUpdates = new();

    private bool _isEliminated;

    public bool IsEliminated => _isEliminated;

    
    public void MarkUpdateReceived(string taskName, System.Text.Json.JsonElement info)
    {
        _taskUpdates[taskName] = info;
    }

    public bool HasSentUpdate(string taskName)
    {
        return _taskUpdates.TryGetValue(taskName, out var value) && value.ValueKind != System.Text.Json.JsonValueKind.Undefined;;
    }
    public void Eliminate()
    {
        _isEliminated = true;
        Console.WriteLine($"{player.Name} has been eliminated from the game.");
        _ = GameMessageSender.SendEliminatedPlayer(player);
    }
    public System.Text.Json.JsonElement GetInfoFrom(string taskName)
    {
        _taskUpdates.TryGetValue(taskName, out var value);
        return value!;
    }
    
}