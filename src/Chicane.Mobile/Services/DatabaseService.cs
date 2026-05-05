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
