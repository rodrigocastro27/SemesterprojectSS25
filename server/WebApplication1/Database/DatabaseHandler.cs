using System.Data.SQLite;
using WebApplication1.Data;

namespace WebApplication1.Database;

public class DatabaseHandler
{
    public static DatabaseHandler Instance { get; } = new DatabaseHandler();

    private DatabaseHandler() { }

    private static SQLiteConnection GetDBConnection()
    {
        return SQLiteConnector.GetConnection();
    }

    // INSERT
    public void InsertIntoLobbies(string lobbyId)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("INSERT INTO Lobbies (`name`) VALUES (@lobbyId);", conn);
        cmd.Parameters.AddWithValue("@lobbyId", lobbyId);
        cmd.ExecuteNonQuery();
    }


    public void InsertIntoPlayers(string deviceId, string username)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("INSERT INTO Players (id, username, isOnline) VALUES (@id, @username, 1);", conn);
        cmd.Parameters.AddWithValue("@id", deviceId);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.ExecuteNonQuery();
    }


    public void InsertIntoLobbyPlayers(string lobbyId, string username, string nickname, bool isHost, string role)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("INSERT INTO LobbyPlayers (Lobby, Player, Nickname, IsHost, Role) VALUES(@lobbyId, @username, @nickname, @isHost, @role);", conn);
        cmd.Parameters.AddWithValue("@lobbyId", lobbyId);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@nickname", nickname);
        cmd.Parameters.AddWithValue("@isHost", isHost);
        cmd.Parameters.AddWithValue("@role", role);
        cmd.ExecuteNonQuery();
    }

    // SELECT
    public string SelectLobbyFromLobbyPlayers(string username)
    {
        string? lobbyId = null!;

        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("SELECT Lobby FROM LobbyPlayers WHERE Player = @username;", conn);
        cmd.Parameters.AddWithValue("@username", username);
        using var reader = cmd.ExecuteReader();

        if (reader.Read())
            lobbyId = reader.IsDBNull(0) ? null : reader.GetString(0);
        return lobbyId!;
    }


    public bool SelectIsHostFromLobbyPlayers(string username)
    {
        bool isHost = false;

        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("SELECT IsHost FROM LobbyPlayers WHERE Player = @username;", conn);
        cmd.Parameters.AddWithValue("@username", username);
        using var reader = cmd.ExecuteReader();

        if (reader.Read())
            isHost = reader.GetBoolean(0);
        return isHost!;
    }


    // UPDATE
    public void UpdateLobbyPlayersNickname(string username, string nickname, string role)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("UPDATE LobbyPlayers SET Nickname = @nickname, Role = @role WHERE Player = @username;", conn);
        cmd.Parameters.AddWithValue("@nickname", nickname);
        cmd.Parameters.AddWithValue("@role", role);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.ExecuteNonQuery();
    }


    public void UpdatePlayersIsOnline(string username, bool isOnline)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("UPDATE Players SET isOnline = @isOnline WHERE username = @username;", conn);
        cmd.Parameters.AddWithValue("@isOnline", isOnline);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.ExecuteNonQuery();

        if (!isOnline)
        {
            DeleteFromLobbyPlayersPlayer(username);
        }
    }


    public void UpdateLobbyPlayersLobby(string username, string lobbyId, string role)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("UPDATE LobbyPlayers SET Lobby = @lobbyId, Role = @role WHERE Player = @username;", conn);
        cmd.Parameters.AddWithValue("@lobbyId", lobbyId);
        cmd.Parameters.AddWithValue("@role", role);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.ExecuteNonQuery();
    }


    public void UpdateLobbyPlayersHost(string username)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("UPDATE LobbyPlayers SET IsHost = 1 WHERE Player = @username;", conn);
        cmd.Parameters.AddWithValue("@username", username);  // username is the player's identifier
        cmd.ExecuteNonQuery();
    }


    // DELETE
    public void DeleteFromLobbies(string lobbyId)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("DELETE FROM Lobbies WHERE `name` = @lobbyId;", conn);
        cmd.Parameters.AddWithValue("@lobbyId", lobbyId);
        cmd.ExecuteNonQuery();

        DeleteFromLobbyPlayersLobby(lobbyId);
    }


    public void DeleteFromPlayers(string username)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("DELETE FROM Players WHERE username = @username;", conn);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.ExecuteNonQuery();
    }


    public void DeleteFromLobbyPlayersLobbyPlayer(string lobbyId, string username)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("DELETE FROM LobbyPlayers WHERE Lobby = @lobbyId AND Player = @username;", conn);
        cmd.Parameters.AddWithValue("@lobbyId", lobbyId);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.ExecuteNonQuery();
    }

    public void DeleteFromLobbyPlayersLobby(string lobbyId)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("DELETE FROM LobbyPlayers WHERE Lobby = @lobbyId;", conn);
        cmd.Parameters.AddWithValue("@lobbyId", lobbyId);
        cmd.ExecuteNonQuery();
    }

    public void DeleteFromLobbyPlayersPlayer(string username)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("DELETE FROM LobbyPlayers WHERE Player = @username;", conn);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.ExecuteNonQuery();
    }
}