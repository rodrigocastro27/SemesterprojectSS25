using WebApplication1.Models;
using WebApplication1.Services;

public abstract class GameTask
{
    public string Name { get; }
    public string Description { get; }

    protected GameTask(string name, string description)
    {
        Name = name;
        Description = description;
    }

    // What happens when task starts
    public abstract Task ExecuteAsync(Lobby lobby);

    public abstract Task EndTask(Lobby lobby, HashSet<PlayerGameSession> respondedSessions);

    // Optional: how to validate completion or broadcast updates
    public virtual Task OnTickAsync(Lobby lobby) => Task.CompletedTask;

    public string GetName() => this.Name;
    public string GetDescription() => this.Description;
}
