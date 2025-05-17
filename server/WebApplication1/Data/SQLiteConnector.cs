namespace WebApplication1.Data;

using System.Data.SQLite;

public class SQLiteConnector
{
    private readonly string _dbPath;
    private readonly string _connectionString;

    public SQLiteConnector()
    {
        _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "game.db");
        _connectionString = $"Data Source={_dbPath};Version=3;";

        EnsureDatabaseExists();
    }

    private void EnsureDatabaseExists()
    {
        if (!File.Exists(_dbPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_dbPath)!);
        SQLiteConnection.CreateFile(_dbPath);
        }
    }

    public SQLiteConnection GetConnection()
    {
        var conn = new SQLiteConnection(_connectionString);
        conn.Open();
        return conn;
    }
}