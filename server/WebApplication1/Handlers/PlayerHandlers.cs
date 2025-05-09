using WebApplication1.Models;

namespace WebApplication1.Handlers;

public static class PlayerHandlers
{
    public static void Register(WebSocketActionDispatcher dispatcher, LobbyManager manager)
    {
        dispatcher.Register("update_position", async (data, socket) =>
        {
            var name = data.GetProperty("name").GetString();
            var lobbyId = data.GetProperty("lobbyId").GetString();
            var lat = data.GetProperty("lat").GetDouble();
            var lon = data.GetProperty("lon").GetDouble();

            var lobby = manager.GetOrCreateLobby(lobbyId);
            var player = lobby.Players.FirstOrDefault(p => p.Name == name);
            
            GeoPosition pos = new GeoPosition(lat, lon);
            player?.UpdateLocation(pos);
        });
        
        
        dispatcher.Register("eliminate_player", async (data, socket) =>
        {
            
        });
    }
}
