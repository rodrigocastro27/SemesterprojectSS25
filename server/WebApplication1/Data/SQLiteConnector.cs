using System.Data.SQLite;

namespace WebApplication1.Data;

public static class SQLiteConnector
{
    private static string _dbPath = null!;
    private static string _connectionString = null!;

    // Static constructor
    public static void Initialize(string contentRootPath)
    {
        _dbPath = Path.Combine(contentRootPath, "Database", "database.db");
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
