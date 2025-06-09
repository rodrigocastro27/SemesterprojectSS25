using WebApplication1.Models;

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

    // Optional: how to validate completion or broadcast updates
    public virtual Task OnTickAsync(Lobby lobby) => Task.CompletedTask;

    public string GetName() => this.Name;
    public string GetDescription() => this.Description;
}
