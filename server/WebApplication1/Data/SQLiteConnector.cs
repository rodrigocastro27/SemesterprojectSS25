using System.Data.SQLite;

namespace WebApplication1.Data;

public static class SQLiteConnector
{
    private static readonly string _dbPath;
    private static readonly string _connectionString;

    // Static constructor
    static SQLiteConnector()
    {
        _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "game.db");
        _connectionString = $"Data Source={_dbPath};Version=3;";

        EnsureDatabaseExists();
    }

    private static void EnsureDatabaseExists()
    {
        if (!File.Exists(_dbPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_dbPath)!);
            SQLiteConnection.CreateFile(_dbPath);
        }
    }

    public static SQLiteConnection GetConnection()
    {
        var conn = new SQLiteConnection(_connectionString);
        conn.Open();
        return conn;
    }
}
