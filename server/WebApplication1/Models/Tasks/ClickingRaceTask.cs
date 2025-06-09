using WebApplication1.Services.Messaging;
using WebApplication1.Models;

public class ClickingRaceTask : GameTask
{
    public ClickingRaceTask() : base("ClickingRace", "Click the button as many times as you can!") { }

    public override async Task ExecuteAsync(Lobby lobby)
    {
        Console.WriteLine("Executing Task: Clicking race.");
    }
}
