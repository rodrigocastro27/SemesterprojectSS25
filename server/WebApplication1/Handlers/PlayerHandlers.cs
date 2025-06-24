using WebApplication1.Database;
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

            Console.WriteLine($"\n[login_player] Player {username} attempting to log in.");

            // // If player exists, create it and add it to database
            var player = PlayerManager.Instance.GetPlayerByName(username!);
            if (player == null)
            {

                Console.WriteLine($"Player {username} is new. Proceeding to creat it.");
                player = PlayerManager.Instance.CreatePlayer(deviceId!, username!, socket);

            }
            else
            {
                Console.WriteLine($"Player is already registered. Proceeding to update its socket.");
                PlayerManager.Instance.UpdatePlayerSocket(username!, socket);
                PlayerManager.Instance.LoginPlayer(username!);
            }

            player.SetOnline(true);

            // Potentially also create a PlayerMessageSender
            await MessageSender.SendToPlayerAsync(player!, "player_registered", new
            {
                id = deviceId,
                user = username
            });
        });

        // dispatcher.Register("update_position", async (data, socket) =>
        // {
        //     // TODO
        // });
        
        
        // dispatcher.Register("eliminate_player", async (data, socket) =>
        // {
        //     // TODO
        // });
    }
}
