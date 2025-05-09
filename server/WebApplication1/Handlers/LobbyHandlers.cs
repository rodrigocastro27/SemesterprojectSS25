using WebApplication1.Models;

namespace WebApplication1.Handlers;

public static class LobbyHandlers
{
    public static void Register(WebSocketActionDispatcher dispatcher, LobbyManager manager)
    {
        dispatcher.Register("join_lobby", async (data, socket) =>
        {
            var name = data.GetProperty("name").GetString();
            var lobbyId = data.GetProperty("lobbyId").GetString();

            var player = new Player(name, socket);
            var lobby = manager.GetOrCreateLobby(lobbyId);
            await lobby.AddPlayerAsync(player);
        });
        
        dispatcher.Register("create_lobby", async (data, socket) =>
        {
            Console.WriteLine("lobby created!");
            
        });
        
        
    }
}
