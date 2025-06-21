using WebApplication1.Services;

namespace WebApplication1.Models.Abilities;

public class GainPingAbility: IAbility
{
    public void Dispose()
    {
        // TODO release managed resources here
    }

    public AbilityType Type { get; }
    public Task ApplyEffectAsync(GameSession session, PlayerGameSession playerSession)
    {
        throw new NotImplementedException();
    }

    public bool CanBeUsed { get; }
}