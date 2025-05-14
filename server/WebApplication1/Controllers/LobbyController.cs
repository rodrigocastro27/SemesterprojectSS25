namespace WebApplication1.Controllers;

using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

[Route("api/[controller]")]
[ApiController]

public class LobbyController : ControllerBase
{
    private readonly IMongoCollection<Lobby>? _lobbies;

    public LobbyController(MongoDBService mongoDBservice)
    {
        _lobbies = mongoDBservice.Database?.GetCollection<Lobby>("lobby");
    }

    // Fetch all lobbies
    [HttpGet("get_lobbies")]
    public async Task<IEnumerable<Lobby>> GetLobby()
    {
        return await _lobbies.Find(FilterDefinition<Lobby>.Empty).ToListAsync();
    }

    // Fetch lobby by id
    [HttpGet("get_lobby_{id}")]
    public async Task<ActionResult<Lobby?>> GetLobbyById(string id)
    {
        var filter = Builders<Lobby>.Filter.Eq(x => x.Id, id);
        var lobby = await _lobbies.Find(filter).FirstOrDefaultAsync();

        return lobby is not null ? Ok(lobby) : NotFound();
    }

    // To create a new lobby
    [HttpPost("create_lobby")]
    public async Task<ActionResult> CreateLobby(Lobby lobby)
    {
        await _lobbies!.InsertOneAsync(lobby);                  // SUPRESSED WARNING
        return CreatedAtAction(nameof(GetLobbyById), new {id = lobby.Id}, lobby);
    }

    // Update existing lobby data
    [HttpPut("update_lobby")]
    public async Task<ActionResult> UpdateLobby(Lobby lobby)
    {
        var filter = Builders<Lobby>.Filter.Eq(x => x.Id, lobby.Id);
        // To update one by one
        // var update = Builders<Lobby>.Update
        //     .Set(x => x.Name, lobby.Name)
        //     .Set(x => x.MaxPlayers, lobby.MaxPlayers);
        // await _lobbies.UpdateOneAsync(filter, update);

        // To update all the fields
        await _lobbies!.ReplaceOneAsync(filter, lobby);     // SUPRESSED WARNING
        return Ok();
    }

    // Delete a Lobby
    [HttpDelete("delete_lobby_{id}")]
    public async Task<ActionResult> DeleteLobby(string id)
    {
        var filter = Builders<Lobby>.Filter.Eq(x => x.Id, id);
        await _lobbies!.DeleteOneAsync(filter);             // SUPRESSED WARNING
        return Ok();
    }
}