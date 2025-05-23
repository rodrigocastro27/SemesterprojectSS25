using WebApplication1.Models;
using WebApplication1.Utils;

namespace WebApplication1.Services.Messaging;

public static class LobbyMessageSender
{
    // [lobby_joined]
    public static async Task JoinedAsync(Lobby lobby, Player player, bool asHost)
    {
        await MessageSender.SendToPlayerAsync(player, "lobby_joined", new
        {
            lobbyId = lobby.Id,
            host = asHost,
            player = new
            {
                name = player.Name,
                role = player.Role,
                nickname = player.Nickname,
            }
        });
    }

    // [new_player_joined]
    public static async Task BroadcastPlayerJoinedAsync(Lobby lobby, Player player)
    {
        await MessageSender.BroadcastLobbyAsync(lobby, "new_player_joined", new
        {
            player = new
            {
                name = player.Name,
                role = player.Role,
                nickname = player.Nickname,
            }
        });
    }

    // [new_host]
    public static async Task SetNewHost(Player player)
    {
        await MessageSender.SendToPlayerAsync(player, "new_host", new
        {
            player = player.Name,
        }); // Notify the handler to send a message to the UI
    }


    // [player_list]
    public static async Task BroadcastPlayerList(Lobby lobby)
    {
        var playersData = lobby.Players.Select(p => new
        {
            name = p.Name, //maybe only name is needed  
            role = p.Role,
            id = p.Id,
            nickname = p.Nickname,
        }).ToList();

        await MessageSender.BroadcastLobbyAsync(lobby, "player_list", new
        {
            players = playersData
        });
    }


    // [lobby_created]
    public static async Task CreatedLobbyAsync(Lobby lobby, Player player)
    {
        await MessageSender.SendToPlayerAsync(player, "lobby_created", new
        {
            lobbyId = lobby.Id,
            player = new
            {
                name = player.Name,
                role = player.Role,
                nickname = player.Nickname,
            }
        });
    }
    
    
    // [leave_lobby]
    public static async Task LeaveAsync(Lobby lobby, Player player)
    {
        await MessageSender.SendToPlayerAsync(player, "leave_lobby", new
        {
            lobby = lobby.Id,
            player = new
            {
                name = player!.Name,
                role = player.Role,
                nickname = player.Nickname,
            }
        });
    }


    // [lobby_deleted]
    public static async Task DeletedLobby(Lobby lobby)
    {
        await MessageSender.BroadcastLobbyAsync(lobby, "lobby_deleted", new
        {
            action = "deleted"
        });
    }

    
    // Errors
    public static async Task ErrorMessageAsync(string lobbyId, Player player, int errorCode)
    {
        switch (errorCode)
        {
            case 1:
                await MessageSender.SendToPlayerAsync(player, "error_creating_lobby", new
                {
                    action = "error_creating_lobby",
                    lobbyId = lobbyId,
                });
                break;
            case 2:
                await MessageSender.SendToPlayerAsync(player, "player_already_in_lobby", new
                {
                    username = player.Name,
                    lobbyId = lobbyId,
                });
                break;
            case 3:
                await MessageSender.SendToPlayerAsync(player, "lobby_does_not_exist", new
                {
                    username = player.Name,
                    lobbyId = lobbyId,
                });
                break;
            default:
                await MessageSender.SendToPlayerAsync(player, "error", new
                {
                    action = "error",
                    lobbyId = lobbyId,
                });
                break;
        }
    }


    // [game_started]
    public static async Task StartGame(Lobby lobby)
    {
        await MessageSender.BroadcastLobbyAsync(lobby, "start_game", new
        {
            action = "started",
        });
    }
}