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
        AbilityType.GainPing,
        AbilityType.HiderSound
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
        var ability = _seekersAbilities[index];
    
        //var ability = CreateAbilityInstance(AbilityType.GainPing);


        gameSession.GrantAbility(CreateAbilityInstance(ability)!);
        _ = GameMessageSender.NotifyAbilityGainAsync(player, ability);  //hard code change to random
    }    
    
    public static void GrantHiderAbility(Player player,  PlayerGameSession gameSession)
    {
        Random rnd = new Random();
        int index = rnd.Next(0, _hidersAbilities.Count);
        var ability = _hidersAbilities[index];
        
        //var ability = CreateAbilityInstance(AbilityType.HidePing);  // hard code

        gameSession.GrantAbility(CreateAbilityInstance(ability)!);
        _ = GameMessageSender.NotifyAbilityGainAsync(player, ability);
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