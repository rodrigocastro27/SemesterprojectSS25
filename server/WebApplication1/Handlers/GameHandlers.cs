using WebApplication1.Services;
using WebApplication1.Services.Messaging;
using WebApplication1.Utils;

namespace WebApplication1.Handlers;

public static class GameHandlers
{
    public static void Register(WebSocketActionDispatcher dispatcher)
    {
        dispatcher.Register("start_game", async (data, socket) =>
        {
            var lobbyId = data.GetProperty("lobbyId").GetString();

            Console.WriteLine($"\n[start_game] Proceeding to start game for lobby {lobbyId}.");

            var lobby = LobbyManager.Instance.GetLobby(lobbyId!);

            Console.WriteLine("Notifying all players in the lobby that the game is starting.");
            await GameMessageSender.SendGameStarted(lobby!, "started");
        });
    }
}