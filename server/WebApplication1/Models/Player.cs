using System.Net.WebSockets;

namespace WebApplication1.Models;

public class Player
{
    public string Name { get; set; }
    public int Id { get; set; } //device id
    public WebSocket Socket { get; set; }
    
    public GeoPosition Position { get; private set; }
    public void UpdateLocation(GeoPosition pos) => Position = pos;
    public Player(string name, WebSocket socket)
    {
        Name = name;
        Socket = socket;
    }

    
}

public struct GeoPosition(double lat, double lon)
{
    public double Latitude { get; } = lat;
    public double Longitude { get; } = lon;
}
