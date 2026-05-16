using Chicane.Mobile.Models.Storage;

namespace Chicane.Mobile.Services.Repositories;

public class RaceWeekendRepository
{
    private readonly DatabaseService _db;

    public RaceWeekendRepository(DatabaseService db) => _db = db;

    public async Task<List<RaceWeekendRecord>> GetAllAsync()
    {
        var conn = await _db.GetConnectionAsync();
        return await conn.Table<RaceWeekendRecord>()
            .OrderBy(r => r.DateStart)
            .ToListAsync();
    }

    public async Task UpsertAsync(List<RaceWeekendRecord> records)
    {
        var conn = await _db.GetConnectionAsync();
        await conn.DeleteAllAsync<RaceWeekendRecord>();
        foreach (var r in records)
            await conn.InsertAsync(r);
    }
}
