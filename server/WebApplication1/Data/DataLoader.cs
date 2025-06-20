using System.Data.SQLite;
using WebApplication1.Database;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Data
{
    /// <summary>
    /// Responsible for loading application state from the SQLite database into memory at startup.
    /// </summary>
    public static class DataLoader
    {
        /// <summary>
        /// Loads all relevant data (lobbies, players, and lobby-player mappings) from the database.
        /// Should be called once at application startup.
        /// </summary>
        public static void LoadAll()
        {
            LoadLobbies();
            LoadPlayers();
            LoadLobbyPlayers();
        }

        /// <summary>
        /// Loads all lobbies from the "Lobbies" table and registers them with the LobbyManager.
        /// </summary>
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

        /// <summary>
        /// Loads all players from the "Players" table and registers them with the PlayerManager.
        /// Sets their online status based on the stored value.
        /// </summary>
        private static void LoadPlayers()
        {
            using var conn = SQLiteConnector.GetConnection();
            var cmd = new SQLiteCommand("SELECT id, username, email, isOnline FROM Players;", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string id = reader.GetString(0);
                string username = reader.GetString(1);
                string? email = reader.IsDBNull(2) ? null : reader.GetString(2);
                bool isOnline = reader.GetBoolean(3);

                var player = new Player(username, "none", id, null!);
                player.SetOnline(isOnline);

                PlayerManager.Instance.AddPlayer(username, player);
            }
        }

        /// <summary>
        /// Loads lobby-player associations from the "LobbyPlayers" table.
        /// Reconnects players to their lobbies if they're online.
        /// If the player is offline, the association is deleted from the table.
        /// In the end, deletes all lobbies that do not have any (online) players.
        /// </summary>
        private static void LoadLobbyPlayers()
        {
            using var conn = SQLiteConnector.GetConnection();
            var cmd = new SQLiteCommand("SELECT Lobby, Player, Nickname, IsHost, Role FROM LobbyPlayers;", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string lobbyId = reader.GetString(0);
                string username = reader.GetString(1);
                string nickname = reader.GetString(2);
                bool isHost = reader.GetBoolean(3);
                string role = reader.GetString(4);

                var player = PlayerManager.Instance.GetPlayer(username);
                if (player == null)
                {
                    Console.WriteLine($"Warning: Player '{username}' not found in PlayerManager.");
                    continue;
                }

                var lobby = LobbyManager.Instance.GetLobby(lobbyId);
                if (lobby == null)
                {
                    Console.WriteLine($"Warning: Lobby '{lobbyId}' not found in LobbyManager.");
                    continue;
                }

                if (!player.IsOnline())
                {
                    var cmd1 = new SQLiteCommand("DELETE FROM LobbyPlayers WHERE Player = @username;", conn);
                    cmd1.Parameters.AddWithValue("@username", username);
                    cmd1.ExecuteNonQuery();
                    continue;
                }

                if (!lobby.HasPlayer(player))
                {
                    lobby.AddPlayer(player);
                }

                if (isHost)
                {
                    player.SetHost(true);
                }

                player.SetNickname(nickname);
                player.SetRole(role);
            }

            LobbyManager.Instance.DeleteEmptyLobbies();
        }
    }
}