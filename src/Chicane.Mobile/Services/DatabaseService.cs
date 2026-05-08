using Chicane.Mobile.Models.Storage;
using SQLite;

namespace Chicane.Mobile.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _connection;
    private readonly string _dbPath;

    public DatabaseService()
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "chicane.db");
    }

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

    public void ClearAll()
    {
        _connection = null;
        try { File.Delete(_dbPath); } catch { }
    }

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_connection is not null)
            return _connection;

        _connection = new SQLiteAsyncConnection(_dbPath);

        await _connection.CreateTableAsync<StandingsRecord>();
        await _connection.CreateTableAsync<ConstructorStandingsRecord>();
        await _connection.CreateTableAsync<RaceScheduleRecord>();
        await _connection.CreateTableAsync<TeamRecord>();

        return _connection;
    }
}
