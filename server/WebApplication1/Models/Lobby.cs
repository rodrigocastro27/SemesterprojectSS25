namespace WebApplication1.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

public class Lobby
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; } 

    public required string Name { get; set; }
    public List<string> PlayerIds { get; set; } = new();
    public int MaxPlayers { get; set; }
    public required string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
