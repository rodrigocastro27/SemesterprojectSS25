using WebApplication1.Data;
using MongoDB.Driver;
using System.Net.WebSockets;
using Fleck;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main(string[] args)
    {
        // Start WebSocket Server using Fleck
        var wsConnections = new List<IWebSocketConnection>();
        var server = new WebSocketServer("ws://0.0.0.0:8181");

        server.Start(ws =>
        {
            ws.OnOpen = () =>
            {
                Console.WriteLine($"Client connected: {ws.ConnectionInfo.ClientIpAddress}");
                wsConnections.Add(ws);
            };

            ws.OnMessage = message =>
            {
                Console.WriteLine($"Received: {message}");
                foreach (var socket in wsConnections)
                {
                    socket.Send($"Echo: {message}");
                }
            };

            ws.OnClose = () =>
            {
                Console.WriteLine($"Client disconnected: {ws.ConnectionInfo.ClientIpAddress}");
                wsConnections.Remove(ws);
            };
        });

        // Build and run ASP.NET Core app
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<IMongoClient>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration["MongoDb:ConnectionString"];
            return new MongoClient(connectionString);
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        // Add services to the container
        builder.Services.AddSingleton<MongoDBService>();
        
        builder.Services.AddControllers(); // Enables [ApiController]
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Enable Swagger only in development (or always, for testing)
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(); // This enables the /swagger page
        }

        

        app.UseCors("AllowAll");
        app.UseAuthorization();
        // Map API controllers
        app.MapControllers();

        app.Run(); // Start HTTP server
    }
}
