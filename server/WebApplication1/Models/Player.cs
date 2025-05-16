using System.Net.WebSockets;

<<<<<<< HEAD
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
=======
namespace WebApplication1.Models;

public class Player(string name, int id, WebSocket socket)
{
    public string Name { get; set; } = name;
    public int Id { get; set; } = id; //device id
    public WebSocket Socket { get; set; } = socket;

    private bool _isHost;
    public GeoPosition Position { get; private set; }
    public string Role = "hider";

    public void UpdateLocation(GeoPosition pos) => Position = pos;
    
    public void SetHost(bool state)=> _isHost = state;
}   

public readonly struct GeoPosition(double lat, double lon)
{
    public double Latitude { get; } = lat;
    public double Longitude { get; } = lon;
}
>>>>>>> FlutterWebSocket
