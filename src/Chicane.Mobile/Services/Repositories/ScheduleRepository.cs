using Chicane.Mobile.Models.Storage;

namespace Chicane.Mobile.Services.Repositories;

public class ScheduleRepository
{
    private readonly DatabaseService _db;

    public ScheduleRepository(DatabaseService db) => _db = db;

    public async Task UpsertAsync(string categoryId, List<RaceScheduleRecord> records)
    {
        var conn = await _db.GetConnectionAsync();
        await conn.ExecuteAsync("DELETE FROM RaceSchedule WHERE CategoryId = ?", categoryId);

        foreach (var record in records)
        {
            await conn.InsertOrReplaceAsync(record);
        }
    }

    public async Task<List<RaceScheduleRecord>> GetByCategoryAsync(string categoryId)
    {
        var conn = await _db.GetConnectionAsync();
        return await conn.Table<RaceScheduleRecord>()
            .Where(r => r.CategoryId == categoryId)
            .OrderBy(r => r.EventDate)
            .ToListAsync();
    }
}
