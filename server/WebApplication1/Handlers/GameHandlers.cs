using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Services.Messaging;
using WebApplication1.Utils;

namespace WebApplication1.Handlers;

public class GameHandlers
{
    public static void Register(WebSocketActionDispatcher dispatcher, LobbyManager manager)
    {
        dispatcher.Register("command1", async (data, socket) =>
        {
            GameMessageSender.SendGameMessage("game id", "Game Message");   //placeholder
        });
        
        
        dispatcher.Register("command2", async (data, socket) =>
        {
            
        });
    }
}