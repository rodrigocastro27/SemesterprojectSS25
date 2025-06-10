using WebApplication1.Services.Messaging;
using WebApplication1.Models;
using WebApplication1.Services;

public class ClickingRaceTask : GameTask
{
    public ClickingRaceTask() : base("ClickingRace", "Click the button as many times as you can!") { }

    private int hidersCounter = 0;
    private int seekersCounter = 0;

    public override async Task ExecuteAsync(Lobby lobby)
    {
        Console.WriteLine($"[ClickingRaceTask] Starting 15-second timer for lobby {lobby.Id}");
        await Task.Delay(TimeSpan.FromSeconds(10));
        Console.WriteLine($"[ClickingRaceTask] Timer ended. Sending message to lobby {lobby.Id}");

        object payload = new
        {
            taskName = "ClickingRace",
            update = new
            {
                type = "time_out",
            },
        };

        await GameMessageSender.BroadcastUpdateTask(lobby, payload);
    }

    public override async Task EndTask(Lobby lobby, HashSet<PlayerGameSession> respondedSessions)
    {
        Console.WriteLine("Ending task...");
        foreach (var session in respondedSessions)
        {
            var info = session.GetInfoFrom("ClickingRace");
            var count = info.GetProperty("count").GetInt32();
            var isHider = info.GetProperty("isHider").GetBoolean();

            if (isHider) hidersCounter += count;
            else seekersCounter += count;
        }

        Console.WriteLine($"RESULT: hiders {hidersCounter} - seekers {seekersCounter}");
        var winners = "";
        if (hidersCounter > seekersCounter) winners = "hiders";
        else if (hidersCounter < seekersCounter) winners = "seekers";
        else winners = "tie";

        Console.WriteLine("Notifying players of the result.");
        await GameMessageSender.BroadcastTaskResult(lobby, winners);

        // Clear all used variables
        foreach (var session in respondedSessions)
        {
            session._taskUpdates.Clear();
        }
        respondedSessions.Clear();
    }
}
