using System.Data.SQLite;

namespace WebApplication1.Data;

/// <summary>
/// Static utility class for initializing and providing access to the SQLite database.
/// </summary>
public static class SQLiteConnector
{
    /// <summary>
    /// Full file path to the SQLite database file.
    /// </summary>
    private static string _dbPath = null!;

    /// <summary>
    /// SQLite connection string built from the database path.
    /// </summary>
    private static string _connectionString = null!;

    /// <summary>
    /// Initializes the SQLite database connection by setting up the path and connection string.
    /// Ensures that the database file and its directory exist.
    /// </summary>
    /// <param name='contentRootPath'>The root directory of the application used to locate the database file.</param>
    public static void Initialize(string contentRootPath)
    {
        _dbPath = Path.Combine(contentRootPath, "Database", "database.db");
        _connectionString = $"Data Source={_dbPath};Version=3;";
        EnsureDatabaseExists();
    }

    /// <summary>
        /// Ensures that the SQLite database file exists. If it doesn't, creates the file and its containing directory.
        /// </summary>
    private static void EnsureDatabaseExists()
    {
        if (!File.Exists(_dbPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_dbPath)!);
            SQLiteConnection.CreateFile(_dbPath);
        }
    }

    /// <summary>
        /// Creates and opens a new connection to the SQLite database.
        /// </summary>
        /// <returns>An open <see cref="SQLiteConnection"/> instance.</returns>
    public static SQLiteConnection GetConnection()
    {
        var conn = new SQLiteConnection(_connectionString);
        conn.Open();
        return conn;
    }
}
