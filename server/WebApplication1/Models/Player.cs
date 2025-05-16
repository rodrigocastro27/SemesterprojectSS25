using System.Net.WebSockets;

public class Player
{
    public string Name { get; set; }
    public WebSocket Socket { get; set; }

    public Player(string name, WebSocket socket)
    {
        Name = name;
        Socket = socket;
    }
}