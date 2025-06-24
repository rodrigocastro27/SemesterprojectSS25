using WebApplication1.Services;

namespace WebApplication1.Models.Abilities;

public class SwapQrAbility : IAbility
{
    public AbilityType Type { get; } = AbilityType.SwapQr;
    public Task ApplyEffectAsync(GameSession session, PlayerGameSession playerSession)
    {
        throw new NotImplementedException();
    }

    public bool CanBeUsed { get; }
    public void MarkUsed()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}