using WebApplication1.Services.Messaging;
using WebApplication1.Models;
using WebApplication1.Services;

public class ClickingRaceTask : GameTask
{
    public ClickingRaceTask() : base("ClickingRace", "Click the button as many times as you can!") { }

    private int _hidersCounter = 0;
    private int _seekersCounter = 0;

    public override async Task ExecuteAsync(Lobby lobby)
    {
        _hidersCounter = 0;
        _seekersCounter = 0;
        
        
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

        PlayerGameSession mvpGameSessionH = null;
        PlayerGameSession mvpGameSessionS = null;
        
        foreach (var session in respondedSessions)
        {
            var info = session.GetInfoFrom("ClickingRace");
            var count = info.GetProperty("count").GetInt32();
            var isHider = info.GetProperty("isHider").GetBoolean();

            if (isHider) _hidersCounter += count;
            else _seekersCounter += count;
            
            
            if (count > hidersMVPcount && isHider)
            {
                hidersMVPcount = count;
                hidersMVP = session.GetPlayer();
                
                mvpGameSessionH = session;
            }
            else if (count > seekersMVPcount && !isHider)
            {
                seekersMVPcount = count;
                seekersMVP = session.GetPlayer();
                
                mvpGameSessionS = session;
                
            }
        }

        var result = _hidersCounter > _seekersCounter ? "hiders"
            : _seekersCounter > _hidersCounter ? "seekers"
            : "tie";

        Console.WriteLine($"RESULT: hiders {_hidersCounter} - seekers {_seekersCounter}");
      
        Console.WriteLine("Notifying players of the result.");
        
        
        await GameMessageSender.BroadcastTaskResult(lobby, result);
        
        if (hidersMVP != null || seekersMVP != null)
        {
            switch (result)
            {
                case "hiders" when hidersMVP != null:
                    Console.WriteLine($"Notifying hider {hidersMVP.Name} that he won an ability!");
                    if (mvpGameSessionH != null) AbilityManager.GrantHiderAbility(hidersMVP, mvpGameSessionH);
                    break;
                case "seekers" when seekersMVP != null:
                    Console.WriteLine($"Notifying seeker {seekersMVP.Name} that he won an ability!");
                    if (mvpGameSessionS != null) AbilityManager.GrantSeekerAbility(seekersMVP, mvpGameSessionS);
                    break;
                default:
                    Console.WriteLine("Inconclusive result. Either tie or uncontrolled case.");
                    break;
            }
        }
        
        foreach (var session in respondedSessions)
            session.TaskUpdates.Clear();
        
        
        respondedSessions.Clear();
    }
}