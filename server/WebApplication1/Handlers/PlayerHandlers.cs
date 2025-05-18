using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Utils;

namespace WebApplication1.Handlers;

public static class PlayerHandlers
{
    public static void Register(WebSocketActionDispatcher dispatcher)
    {
        dispatcher.Register("login_player", async (data, socket) =>
        {
            var deviceId = data.GetProperty("deviceId").GetString();
            var username = data.GetProperty("username").GetString();

            PlayerManager.Instance.PrintPlayers();

            // // If player exists, create it and add it to database
            var player = PlayerManager.Instance.GetPlayerByName(username!);
            if (player == null) {
                player = PlayerManager.Instance.CreatePlayer(deviceId!, username!, socket);
            } else {
                PlayerManager.Instance.UpdatePlayerSocket(username!, socket);
            }

            await MessageSender.SendToPlayerAsync(player!, "player_registered", new
                {
                    id = deviceId,
                    user = username
                });
        });

        dispatcher.Register("update_position", async (data, socket) =>
        {
            // TODO
        });
        
        
        dispatcher.Register("eliminate_player", async (data, socket) =>
        {
            // TODO
        });
    }
}
