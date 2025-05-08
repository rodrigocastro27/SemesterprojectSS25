using WebApplication1.Services;
using WebApplication1.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

[Route("api/[controller]")]
[ApiController]

public class LobbyController : ControllerBase
{
    private readonly IMongoCollection<Lobby> _lobbyCollection;
    private readonly IMongoCollection<Player> _playerCollection;

    public LobbyController(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("semprojDB");
        _lobbyCollection = database.GetCollection<Lobby>("Lobbies");
        _playerCollection = database.GetCollection<Player>("Players");
    }

    // Create a new player
    [HttpPost("player/register")]
    public async Task<ActionResult<Player>> RegisterPlayer([FromBody] Player player)
    {
        await _playerCollection.InsertOneAsync(player);
        return Ok(player);
    }

    // Create a new lobby
    [HttpPost("lobby/create")]
    public async Task<ActionResult<Lobby>> CreateLobby([FromBody] Lobby lobby)
    {
        lobby.Status = "waiting";
        lobby.CreatedAt = DateTime.UtcNow;
        await _lobbyCollection.InsertOneAsync(lobby);
        return Ok(lobby);
    }

    // Get list of all lobbies
    [HttpGet("lobbies")]
    public async Task<ActionResult<List<Lobby>>> GetLobbies()
    {
        var lobbies = await _lobbyCollection.Find(_ => true).ToListAsync();
        return Ok(lobbies);
    }

    // Join a lobby
    [HttpPost("lobby/join")]
    public async Task<ActionResult> JoinLobby([FromBody] JoinLobbyRequest request)
    {
        var player = await _playerCollection.Find(p => p.Id == request.PlayerId).FirstOrDefaultAsync();
        if (player == null) return NotFound("Player not found");

        var lobby = await _lobbyCollection.Find(l => l.Id == request.LobbyId).FirstOrDefaultAsync();
        if (lobby == null) return NotFound("Lobby not found");

        if (lobby.PlayerIds.Count >= lobby.MaxPlayers) return BadRequest("Lobby is full");

        lobby.PlayerIds.Add(request.PlayerId);
        await _lobbyCollection.ReplaceOneAsync(l => l.Id == lobby.Id, lobby);

        player.Status = "in_lobby";
        player.CurrentLobbyId = lobby.Id;
        await _playerCollection.ReplaceOneAsync(p => p.Id == player.Id, player);

        return Ok(lobby);
    }


    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("pong");
    }
}

public class JoinLobbyRequest
{
    public required string PlayerId { get; set; }
    public required string LobbyId { get; set; }
}

