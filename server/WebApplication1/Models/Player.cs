namespace WebApplication1.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Player
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }

    public required string Username { get; set; }
    public required string Status { get; set; }
    public string? CurrentLobbyId { get; set; }
}