namespace WebApplication1.Data;

using MongoDB.Driver;
using System;

public class MongoDBService
{
    private readonly IConfiguration _configuration;
    private readonly IMongoDatabase? _database;

    public MongoDBService(IConfiguration configuration)
    {
        _configuration = configuration;

        var connectionString = _configuration["MongoDb:ConnectionString"];
        var databaseName = _configuration["MongoDb:DatabaseName"];

        Console.WriteLine($"CONN STRING: {connectionString}");
        Console.WriteLine($"DB NAME: {databaseName}");

        var mongoClient = new MongoClient(connectionString);
        _database = mongoClient.GetDatabase(databaseName);
    }

    public IMongoDatabase? Database => _database;
}