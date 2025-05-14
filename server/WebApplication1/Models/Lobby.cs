namespace WebApplication1.Models
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using System;

    public class Lobby
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }

        [BsonElement("name")]
        [BsonRepresentation(BsonType.String)]
        public required string Name { get; set; }

        [BsonElement("max_players")]
        [BsonRepresentation(BsonType.Int32)]
        public int MaxPlayers { get; set; }
    }
}
