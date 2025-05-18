using System.Data.SQLite;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Data;

public static class DataLoader
{
    public static void LoadAll()
    {
        LoadLobbies();
        LoadPlayers();
    }

    private static void LoadLobbies()
    {
        using var conn = SQLiteConnector.GetConnection();
        var cmd = new SQLiteCommand("SELECT `name`, max_players FROM Lobbies;", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            string name = reader.GetString(0);
            int maxPlayers = reader.GetInt32(1);

            var lobby = new Lobby(name);
            LobbyManager.Instance.AddLobby(name, lobby);
        }
    }

    private static void LoadPlayers()
    {
        using var conn = SQLiteConnector.GetConnection();
        var cmd = new SQLiteCommand("SELECT id, username, email, lobbyId FROM Players;", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            string id = reader.GetString(0);
            string username = reader.GetString(1);
            string? email = reader.IsDBNull(2) ? null : reader.GetString(2);
            string? lobbyId = reader.IsDBNull(3) ? null : reader.GetString(3);

            var player = new Player(username, id, null!);

            PlayerManager.Instance.AddPlayer(username, player);

            if (lobbyId != null)
            {
                var lobby = LobbyManager.Instance.GetLobby(lobbyId);
                lobby?.AddPlayer(player);
            }
        }
    }
}
