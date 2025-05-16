using WebApplication1.Models;

namespace WebApplication1.Handlers;

public interface IHandler
{
    public static abstract void Register(WebSocketActionDispatcher dispatcher);
}