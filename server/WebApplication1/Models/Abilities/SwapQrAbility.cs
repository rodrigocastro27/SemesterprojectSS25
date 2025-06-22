using WebApplication1.Services;
using WebApplication1.Services.Messaging;

namespace WebApplication1.Models.Abilities;

public class SwapQrAbility : IAbility
{
    public AbilityType Type { get; } = AbilityType.SwapQr;
    
    private TaskCompletionSource<bool>? _targetPlayerEliminated;
    private bool _used = false;

    private PlayerGameSession _currentPlayerGameSession;
    private string fakeName;
    private Player? _targetSeeker;
    private GameSession? _session;


    
    public async Task ApplyEffectAsync(GameSession session, PlayerGameSession playerSession)
    {
        
        _session = session;
        _targetPlayerEliminated = new TaskCompletionSource<bool>();
        
        //get a new code
        var seekers = session.GetLobby().GetSeekerList();
        int random = new Random().Next(0, seekers.Count);
        
        _targetSeeker = seekers[random];
        fakeName = _targetSeeker.Name;
        
        //inform player of their new qr code.
        _ = GameMessageSender.NotifyNewQrCode(playerSession.GetPlayer(), fakeName);


        
        // Register interceptor
        session.RegisterEliminationInterceptor(InterceptElimination);
        
        //inform player the qr code was scanned (ability used)
        _ = GameMessageSender.NotifyQrCodeScanned(playerSession.GetPlayer());
    }
    
    public bool CanBeUsed => !_used;

    private async Task<bool> InterceptElimination(Player scannedPlayer)
    {
        if (_used || _session == null || _targetSeeker == null) return false;

        // If the player being eliminated has our fake QR, we intercept
        if (scannedPlayer == _targetSeeker)
        {

            // Notify the hider their ability triggered
            await GameMessageSender.NotifyQrCodeScanned(_currentPlayerGameSession.GetPlayer());

            _used = true;
            Dispose(); // Clean up
            return true; // Cancel original elimination
        }

        return false; // Let elimination proceed
    }


    public void Dispose()
    {
        if (_currentPlayerGameSession.HasAbility(this))
        {
            _currentPlayerGameSession.GetAbilityList().Remove(this);
            _session?.UnregisterEliminationInterceptor(InterceptElimination);
            Console.WriteLine("SwitchQrAbility disposed.");
        }
    }
}