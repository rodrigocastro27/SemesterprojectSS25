using System.Data.SQLite;
using WebApplication1.Database;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Data
{
    public static class DataLoader
    {
        public static void LoadAll()
        {
            LoadLobbies();
            LoadPlayers();
            LoadLobbyPlayers();
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