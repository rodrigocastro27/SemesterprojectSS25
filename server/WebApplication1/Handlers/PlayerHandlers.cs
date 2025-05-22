using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Utils;

namespace WebApplication1.Handlers;

public static class PlayerHandlers
{
    public static void Register(WebSocketActionDispatcher dispatcher)
    {
        dispatcher.Register("update_position", async (data, socket) =>
        {
 
        });
        
        
        dispatcher.Register("eliminate_player", async (data, socket) =>
        {
            
        });
    }
}
