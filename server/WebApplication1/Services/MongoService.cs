namespace WebApplication1.Services;

using MongoDB.Driver;
using WebApplication1.Models;
using System;

public class MongoService
{
    private readonly IMongoDatabase _database;

    public IMongoCollection<Lobby> Lobbies => _database.GetCollection<Lobby>("Lobbies");
    public IMongoCollection<Lobby> Players => _database.GetCollection<Lobby>("Players");

    public MongoService(IConfiguration configuration)
    {

        // Get connection string and database name from appsettings
        var connectionString = configuration["MongoDb:ConnectionString"];
        var databaseName = configuration["MongoDb:DatabaseName"];

        // Create a MongoClient instance
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

}
