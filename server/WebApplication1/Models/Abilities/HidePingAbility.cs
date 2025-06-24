using WebApplication1.Services;

namespace WebApplication1.Models.Abilities;

public class HidePingAbility : IAbility
{
    public AbilityType Type { get; } = AbilityType.HidePing;    //INITIALIZE TYPE
    
    
    private PlayerGameSession _currentPlayerGameSession;
    public async Task ApplyEffectAsync(GameSession session, PlayerGameSession playerSession)
    {
        playerSession.SetVisible(false);

        await session.HandlePing();

        playerSession.SetVisible(true);
    }
    public bool CanBeUsed { get; }

    public void Dispose()
    {
        if (_currentPlayerGameSession.HasAbility(this))
        {
            _currentPlayerGameSession.GetAbilityList().Remove(this);
            Console.WriteLine("HidePingAbility disposed.");
        }
    }
}