    using WebApplication1.Utils;
    using WebApplication1.Models;
    using Microsoft.AspNetCore.Identity;

    namespace WebApplication1.Services.Messaging;

    public static class GameMessageSender
    {
        public static async Task SendGameStarted(Lobby lobby)
        {
            await MessageSender.BroadcastLobbyAsync(lobby!, "game_started", new
            {
            });
        }
        public static async Task RequestHidersLocation(Lobby lobby)
        {
            await MessageSender.BroadcastToHidersAsync(lobby, "location_request", new
            {
                //idk what else to send
            });
        }

        public static Task SendPingToSeekers(Lobby lobby, List<Player> updatedLocations)
        {
            var playersData = updatedLocations.Select(p => new
            {
                name = p.Name,
                id = p.Id,
                lat = p.Position.Latitude,
                lon = p.Position.Longitude,
            });

            return MessageSender.BroadcastToSeekersAsync(lobby, "location_update_list", new
            {
                players = playersData
            });
        }

        public static async Task SendGameEnded(Lobby lobby)
        {
            await MessageSender.BroadcastLobbyAsync(lobby, "game_ended", new
            {
                //who won, other relevant info
            });
        }

        public static async Task SendTimeUpdate(Lobby lobby, TimeSpan time)
        {
            DateTime now = DateTime.Now;
            await MessageSender.BroadcastLobbyAsync(lobby, "time_update", new
            {
                time = time,
                time_offset = now,
            });
        }

        #region Task Messages

        public static async Task BroadcastTask(Lobby lobby, GameTask task)
        {
            Console.WriteLine($"Notifying players in lobby {lobby.Id} that the task is starting.");
            await MessageSender.BroadcastLobbyAsync(lobby, "task_started", new
            {
                name = task.GetName(),
            });
        }

        public static async Task BroadcastUpdateTask(Lobby lobby, object payload)
        {
            Console.WriteLine($"Updating task for lobby {lobby.Id}.");
            await MessageSender.BroadcastLobbyAsync(lobby, "task_update", payload);
        }

        public static async Task BroadcastTaskResult(Lobby lobby, string winners)
        {
            await MessageSender.BroadcastLobbyAsync(lobby, "task_result", new
            {
                winners = winners,
            });
        }

        #endregion
        public static async Task BroadcastEliminatedPlayer(Lobby lobby, Player player)
        {
            await MessageSender.BroadcastLobbyAsync(lobby, "eliminated_player", new
            {
                player.Id,
            });
        }
        
        public static async Task SendEliminatedPlayer(Player player)
        {
            Console.WriteLine($"Informing {player.Name} they are eliminated.");
            await MessageSender.SendToPlayerAsync(player, "current_player_eliminated", new
            {
                
            });
        }
        public static async Task NotifyAbilityGainAsync(Player player, string role)
        {
            await MessageSender.SendToPlayerAsync(player, "gained_ability", new
            {
                role = role,
            });
        }
    }