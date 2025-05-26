using System.Net.WebSockets;

namespace WebApplication1.Models;

public class Player(string name, string nickname, string deviceId, WebSocket socket)
{
    public string Name { get; set; } = name;
    public string Nickname { get; private set; } = nickname;
    public string Id { get; set; } = deviceId; //device id
    public WebSocket Socket { get; set; } = socket;

    public bool IsHost;
    public GeoPosition Position { get; private set; }
    public string Role = "hider";

    private Role RoleEnum { get; set; }

    public void UpdateLocation(GeoPosition pos) => Position = pos;

    public void SetHost(bool state) => IsHost = state;

    public void SetNickname(string nickname) => Nickname = nickname;

    
    
    public void SetRole(Role role) => RoleEnum = role;
    public void SetRole(string role) => RoleEnum = Enum.Parse<Role>(role);
    public string GetRole_s() => nameof(RoleEnum);
    public Role GetRole()=> RoleEnum;
}

public readonly struct GeoPosition(double lat, double lon)
{
    public double Latitude { get; } = lat;
    public double Longitude { get; } = lon;
}

public enum Role
{
    hider,
    seeker
}