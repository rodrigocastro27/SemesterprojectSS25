using WebApplication1.Models;
using WebApplication1.Models.Abilities;
using WebApplication1.Services.Messaging;
using WebApplication1.Utils;

namespace WebApplication1.Services;

public static class AbilityManager
{
    private static List<AbilityType> _hidersAbilities =
    [
        AbilityType.SwapQr,
        AbilityType.HidePing
    ];
    
   private static List<AbilityType> _seekersAbilities =
    [
        AbilityType.SwapQr,
        AbilityType.HidePing
    ];



    public static void GrantRandomAbility(Player player)
    {
        AbilityType ability;
        Random rnd = new Random();
        
        if (player.GetRole() == Role.hider)
        {
            int index = rnd.Next(0, _hidersAbilities.Count);
            ability = _hidersAbilities[index];
            _ = GameMessageSender.NotifyAbilityGainAsync(player, ability);
        }else if (player.GetRole() == Role.seeker)
        {
            int index = rnd.Next(0, _seekersAbilities.Count);
            ability = _seekersAbilities[index];
            _ = GameMessageSender.NotifyAbilityGainAsync(player, ability);
        }
    }

    
    public static void GrantSeekerAbility(Player player, PlayerGameSession gameSession)
    {
        Random rnd = new Random();
        int index = rnd.Next(0, _seekersAbilities.Count);
  //      var ability = _seekersAbilities[index];
    
            var ability = CreateAbilityInstance(AbilityType.GainPing);


            if (ability != null) gameSession.GrantAbility(ability);
            _ = GameMessageSender.NotifyAbilityGainAsync(player, AbilityType.GainPing);  //hard code change to random
    }    
    
    public static void GrantHiderAbility(Player player,  PlayerGameSession gameSession)
    {
        Random rnd = new Random();
        int index = rnd.Next(0, _hidersAbilities.Count);
//        var ability = _hidersAbilities[index];
        
        var ability = CreateAbilityInstance(AbilityType.HidePing);

        if (ability != null) gameSession.GrantAbility(ability);
        _ = GameMessageSender.NotifyAbilityGainAsync(player, AbilityType.HidePing);  //hard code change to random
    }
    
    private static IAbility? CreateAbilityInstance(AbilityType type)
    {
        return type switch
        {
            AbilityType.SwapQr => new SwapQrAbility(),
            AbilityType.HidePing => new HidePingAbility(),
            AbilityType.GainPing => new GainPingAbility(),
            AbilityType.HiderSound => new HiderSoundAbility(),
            _ => null
        };
    }

    
}