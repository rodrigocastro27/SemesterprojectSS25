using WebApplication1.Services;

namespace WebApplication1.Models.Abilities;

public interface IAbility : IDisposable
{
    AbilityType Type { get; }
    Task ApplyEffectAsync(GameSession session, PlayerGameSession playerSession);
    bool CanBeUsed { get; }
}

public enum AbilityType
{
    GainPing,
    SwapQr,
    HiderSound,
    HidePing,
}
