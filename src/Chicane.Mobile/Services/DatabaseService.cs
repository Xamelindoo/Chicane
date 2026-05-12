using Chicane.Mobile.Models.Storage;
using SQLite;

namespace Chicane.Mobile.Services;

/// <summary>
/// Manages the local SQLite database (chicane.db).
/// Provides connection creation, table setup, and schema migrations.
/// Uses a versioned migration system (db_schema_version in Preferences) to safely
/// add columns without destroying existing cached data.
/// </summary>
public class DatabaseService
{
    private SQLiteAsyncConnection? _connection;
    private readonly string _dbPath;

    public DatabaseService()
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "chicane.db");
    }

    /// <summary>
    /// Checks if the database file exists and has the expected tables.
    /// </summary>
    public async Task<bool> IsFirstLaunchOrCacheClearedAsync()
    {
        try
        {
            if (!File.Exists(_dbPath))
                return true;

            var conn = new SQLiteAsyncConnection(_dbPath);
            var tableInfo = await conn.QueryAsync<object>("SELECT name FROM sqlite_master WHERE type='table' AND name='RaceSchedule'");
            return tableInfo.Count == 0;
        }
        catch
        {
            return true;
        }
    }

    /// <summary>
    /// Deletes the entire database file. Used for forced cache resets.
    /// </summary>
    public void ClearAll()
    {
        _connection = null;
        try { File.Delete(_dbPath); } catch { }
    }

    /// <summary>
    /// Returns the singleton SQLite connection, creating tables on first access.
    /// All 6 domain tables are created here (Standings, ConstructorStandings, Schedule, Teams, SessionResults, RaceWeekends).
    /// </summary>
    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_connection is not null)
            return _connection;

        _connection = new SQLiteAsyncConnection(_dbPath);

        // Core data tables
        await _connection.CreateTableAsync<StandingsRecord>();
        await _connection.CreateTableAsync<ConstructorStandingsRecord>();
        await _connection.CreateTableAsync<RaceScheduleRecord>();
        await _connection.CreateTableAsync<TeamRecord>();
        // Detail/card tables
        await _connection.CreateTableAsync<SessionResultRecord>();
        await _connection.CreateTableAsync<RaceWeekendRecord>();

        return _connection;
    }

    /// <summary>
    /// Runs schema migrations based on the stored version in Preferences.
    /// v2: added HeadshotUrl column to DriverStandings table.
    /// Migration is fire-and-forget at app startup — non-blocking.
    /// </summary>
    public async Task MigrateAsync()
    {
        var version = Preferences.Get("db_schema_version", 0);
        var conn = await GetConnectionAsync();

        if (version < 2)
        {
            try
            {
                await conn.ExecuteAsync("ALTER TABLE DriverStandings ADD COLUMN HeadshotUrl TEXT DEFAULT ''");
                System.Diagnostics.Debug.WriteLine("[DB] Migration v2: added HeadshotUrl column");
            }
            catch { /* Column already exists — safe to ignore */ }
            Preferences.Set("db_schema_version", 2);
        }
    }
}
