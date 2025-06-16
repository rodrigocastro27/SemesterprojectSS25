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
        hidersCounter = 0;
        seekersCounter = 0;
        
        
        Console.WriteLine($"[ClickingRaceTask] Starting 10-second timer for lobby {lobby.Id}");
        await Task.Delay(TimeSpan.FromSeconds(10));
        Console.WriteLine($"[ClickingRaceTask] Timer ended. Sending message to lobby {lobby.Id}");

        var payload = new
        {
            taskName = "ClickingRace",
            update = new { type = "time_out" }
        };

        await GameMessageSender.BroadcastUpdateTask(lobby, payload);
    }

    public override async Task EndTask(Lobby lobby, HashSet<PlayerGameSession> respondedSessions)
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

        var result = hidersCounter > seekersCounter ? "hiders"
            : seekersCounter > hidersCounter ? "seekers"
            : "tie";

        Console.WriteLine($"RESULT: hiders {hidersCounter} - seekers {seekersCounter}");
      
        Console.WriteLine("Notifying players of the result.");
        
        
        await GameMessageSender.BroadcastTaskResult(lobby, result);
        
        if (hidersMVP != null || seekersMVP != null)
        {
            switch (result)
            {
                case "hiders" when hidersMVP != null:
                    Console.WriteLine($"Notifying hider {hidersMVP.Name} that he won an ability!");
                    await GameMessageSender.NotifyAbilityGainAsync(hidersMVP, "hider");
                    break;
                case "seekers" when seekersMVP != null:
                    Console.WriteLine($"Notifying seeker {seekersMVP.Name} that he won an ability!");
                    await GameMessageSender.NotifyAbilityGainAsync(seekersMVP, "hider");
                    break;
                default:
                    Console.WriteLine("Unconclusive result. Either tie or uncontrolled case.");
                    break;
            }
        }
        
        foreach (var session in respondedSessions)
            session._taskUpdates.Clear();
        
        
        respondedSessions.Clear();
    }
}