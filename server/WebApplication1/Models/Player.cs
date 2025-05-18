using System.Net.WebSockets;

namespace WebApplication1.Models;

public class Player(string name, string deviceId, WebSocket socket)
{
    public string Name { get; set; } = name;
    public string Id { get; set; } = deviceId; //device id
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
