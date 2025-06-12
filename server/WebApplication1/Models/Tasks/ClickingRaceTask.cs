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

        ResetCounters();

        await GameMessageSender.BroadcastUpdateTask(lobby, payload);
    }

    private void ResetCounters()
    {
        hidersCounter = 0;
        seekersCounter = 0;
    }

    public override async Task EndTask(Lobby lobby, List<PlayerGameSession> respondedSessions)
    {
        Console.WriteLine("Ending task...");
        Player? hidersMVP = null;
        int hidersMVPcount = 0;
        Player? seekersMVP = null;
        int seekersMVPcount = 0;

        foreach (var session in respondedSessions)
        {
            var info = session.GetInfoFrom("ClickingRace");
            var count = info.GetProperty("count").GetInt32();
            var isHider = info.GetProperty("isHider").GetBoolean();

            if (isHider) hidersCounter += count;
            else seekersCounter += count;

            if (count > hidersMVPcount && isHider)
            {
                hidersMVPcount = count;
                hidersMVP = session.GetPlayer();
            }
            else if (count > seekersMVPcount && !isHider)
            {
                seekersMVPcount = count;
                seekersMVP = session.GetPlayer();
            }
        }

        Console.WriteLine($"RESULT: hiders {hidersCounter} - seekers {seekersCounter}.");
        var winners = "";
        if (hidersCounter > seekersCounter) winners = "hiders";
        else if (hidersCounter < seekersCounter) winners = "seekers";
        else winners = "tie";

        Console.WriteLine("Notifying players of the result.");
        await GameMessageSender.BroadcastTaskResult(lobby, winners);
        if (hidersMVP != null || seekersMVP != null)
        {
            if (winners == "hiders" && hidersMVP != null)
            {
                Console.WriteLine($"Notifying hider {hidersMVP.Name} that he won an ability!");
                await GameMessageSender.NotifyAbilityGainAsync(hidersMVP, "hider");
            }
            else if (winners == "seekers" && seekersMVP != null)
            {
                Console.WriteLine($"Notifying seeker {seekersMVP.Name} that he won an ability!");
                await GameMessageSender.NotifyAbilityGainAsync(seekersMVP, "hider");
            }
            else
            {
                Console.WriteLine("Unconclusive result. Either tie or uncontrolled case.");
            }
        }

        // Clear all used variables
        foreach (var session in respondedSessions)
        {
            session._taskUpdates.Clear();
        }
        respondedSessions.Clear();
    }
}
