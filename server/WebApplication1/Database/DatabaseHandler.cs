using System.Data.SQLite;
using WebApplication1.Data;

namespace WebApplication1.Database;

/// <summary>
/// Provides CRUD operations for managing the application's database tables.
/// Uses SQLite for persistent storage of lobbies, players, and their relationships.
/// </summary>
public class DatabaseHandler
{
    /// <summary>
    /// Singleton instance of the handler.
    /// </summary>
    public static DatabaseHandler Instance { get; } = new DatabaseHandler();

    private DatabaseHandler() { }

    /// <summary>
    /// Gets a new open SQLite connection using the shared connector.
    /// </summary>
    private static SQLiteConnection GetDBConnection()
    {
        return SQLiteConnector.GetConnection();
    }

    // ========== INSERT ==========

    /// <summary>
    /// Inserts a new lobby record into the Lobbies table.
    /// </summary>
    public void InsertIntoLobbies(string lobbyId)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("INSERT INTO Lobbies (`name`) VALUES (@lobbyId);", conn);
        cmd.Parameters.AddWithValue("@lobbyId", lobbyId);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Inserts a new player record into the Players table with initial online status.
    /// </summary>
    public void InsertIntoPlayers(string deviceId, string username)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("INSERT INTO Players (id, username, isOnline) VALUES (@id, @username, 1);", conn);
        cmd.Parameters.AddWithValue("@id", deviceId);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Adds a player to a lobby with metadata such as nickname, host flag, and role.
    /// </summary>
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

    // ========== SELECT ==========

    /// <summary>
    /// Gets the lobby ID that the given player is currently assigned to.
    /// </summary>
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

    /// <summary>
    /// Checks whether a player is the host of their current lobby.
    /// </summary>
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


    // ========== UPDATE ==========

    /// <summary>
    /// Updates a player's nickname and role in the lobby.
    /// </summary>
    public void UpdateLobbyPlayersNickname(string username, string nickname, string role)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("UPDATE LobbyPlayers SET Nickname = @nickname, Role = @role WHERE Player = @username;", conn);
        cmd.Parameters.AddWithValue("@nickname", nickname);
        cmd.Parameters.AddWithValue("@role", role);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Updates a player's online status. Also removes them from lobby mapping if offline.
    /// </summary>
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

    /// <summary>
    /// Moves a player to a different lobby and updates their role.
    /// </summary>

    public void UpdateLobbyPlayersLobby(string username, string lobbyId, string role)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("UPDATE LobbyPlayers SET Lobby = @lobbyId, Role = @role WHERE Player = @username;", conn);
        cmd.Parameters.AddWithValue("@lobbyId", lobbyId);
        cmd.Parameters.AddWithValue("@role", role);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Promotes a player to be the host of their lobby.
    /// </summary>
    public void UpdateLobbyPlayersHost(string username)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("UPDATE LobbyPlayers SET IsHost = 1 WHERE Player = @username;", conn);
        cmd.Parameters.AddWithValue("@username", username);  // username is the player's identifier
        cmd.ExecuteNonQuery();
    }


    // ========== DELETE ==========

    /// <summary>
    /// Deletes a lobby and all its player associations.
    /// </summary>
    public void DeleteFromLobbies(string lobbyId)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("DELETE FROM Lobbies WHERE `name` = @lobbyId;", conn);
        cmd.Parameters.AddWithValue("@lobbyId", lobbyId);
        cmd.ExecuteNonQuery();

        DeleteFromLobbyPlayersLobby(lobbyId);
    }

    /// <summary>
    /// Deletes a player record.
    /// </summary>
    public void DeleteFromPlayers(string username)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("DELETE FROM Players WHERE username = @username;", conn);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Removes a specific player from a specific lobby.
    /// </summary>
    public void DeleteFromLobbyPlayersLobbyPlayer(string lobbyId, string username)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("DELETE FROM LobbyPlayers WHERE Lobby = @lobbyId AND Player = @username;", conn);
        cmd.Parameters.AddWithValue("@lobbyId", lobbyId);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Removes all players from the specified lobby.
    /// </summary>
    public void DeleteFromLobbyPlayersLobby(string lobbyId)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("DELETE FROM LobbyPlayers WHERE Lobby = @lobbyId;", conn);
        cmd.Parameters.AddWithValue("@lobbyId", lobbyId);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Removes all lobby-player associations for a specific player.
    /// </summary>
    public void DeleteFromLobbyPlayersPlayer(string username)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand("DELETE FROM LobbyPlayers WHERE Player = @username;", conn);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.ExecuteNonQuery();
    }
}