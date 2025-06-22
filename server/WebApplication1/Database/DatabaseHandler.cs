using System.Data.SQLite;
using WebApplication1.Data;
using WebApplication1.Models;

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
    
    public int InsertUser(string email, string passwordHash, string? googleId)
    {
        using var conn = GetDBConnection(); 
        var cmd = new SQLiteCommand(
            "INSERT INTO Users (Email, PasswordHash, GoogleId) VALUES (@email, @passwordHash, @googleId); SELECT last_insert_rowid();", conn);
        cmd.Parameters.AddWithValue("@email", email);
        cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
        cmd.Parameters.AddWithValue("@googleId", (object?)googleId ?? DBNull.Value);
        return (int)(long)cmd.ExecuteScalar(); 
    }
    
    public int InsertPasswordResetToken(int userId, string token, DateTime expiresAt, bool used)
    {
        using var conn = GetDBConnection(); 
        var cmd = new SQLiteCommand(
            "INSERT INTO PasswordResetTokens (UserID, Token, ExpiresAt, Used) VALUES (@userId, @token, @expiresAt, @used); SELECT last_insert_rowid();", conn);
        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Parameters.AddWithValue("@token", token);
        cmd.Parameters.AddWithValue("@expiresAt", expiresAt.ToString("yyyy-MM-dd HH:mm:ss"));
        cmd.Parameters.AddWithValue("@used", used);
        return (int)(long)cmd.ExecuteScalar();
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

    public User? SelectUserById(int userId)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand(
            "SELECT UserID, Email, PasswordHash, GoogleId FROM Users WHERE UserID = @userId;", conn);
        cmd.Parameters.AddWithValue("@userId", userId);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            int retrievedUserId = reader.GetInt32(0);
            string retrievedEmail = reader.GetString(1);
            string storedPasswordHash = reader.GetString(2);
            string? googleId = reader.IsDBNull(3) ? null : reader.GetString(3);
            return new User(retrievedUserId, retrievedEmail, storedPasswordHash, googleId);
        }
        return null;
    }
    
    public User? SelectUserByEmail(string email)
    {
        using var conn = GetDBConnection(); 
        var cmd = new SQLiteCommand(
            "SELECT UserID, Email, PasswordHash, GoogleId FROM Users WHERE Email = @email;", conn);
        cmd.Parameters.AddWithValue("@email", email);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            int retrievedUserId = reader.GetInt32(0);
            string retrievedEmail = reader.GetString(1);
            string storedPasswordHash = reader.GetString(2);
            string? googleId = reader.IsDBNull(3) ? null : reader.GetString(3);
            return new User(retrievedUserId, retrievedEmail, storedPasswordHash, googleId);
        }
        return null;
    }
    
    public User? SelectPasswordResetToken(string token)
    {
        using var conn = GetDBConnection(); 
        var cmd = new SQLiteCommand(
            "SELECT TokenID, UserID, ExpiresAt, Used FROM PasswordResetTokens WHERE Token = @token;", conn);
        cmd.Parameters.AddWithValue("@token", token);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            int tokenId = reader.GetInt32(0);
            int userId = reader.GetInt32(1);
            DateTime expiresAt = DateTime.Parse(reader.GetString(2));
            bool used = reader.GetBoolean(3);

           
            return new User(userId, "", "") 
            {
                TokenID = tokenId, 
                Token = token,
                ExpiresAt = expiresAt,
                Used = used
            };
        }
        return null;
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

    public void UpdateUserPassword(int userId, string newPasswordHash)
    {
        using var conn = GetDBConnection();
        var cmd = new SQLiteCommand(
            "UPDATE Users SET PasswordHash = @newPasswordHash WHERE UserID = @userId;", conn);
        cmd.Parameters.AddWithValue("@newPasswordHash", newPasswordHash);
        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.ExecuteNonQuery();
    }
    
    public void UpdatePasswordResetTokenUsed(int tokenId)
    {
        using var conn = GetDBConnection(); 
        var cmd = new SQLiteCommand(
            "UPDATE PasswordResetTokens SET Used = 1 WHERE TokenID = @tokenId;", conn);
        cmd.Parameters.AddWithValue("@tokenId", tokenId);
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