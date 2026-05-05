using Chicane.Mobile.Models.Storage;

namespace Chicane.Mobile.Services.Repositories;

public class ScheduleRepository
{
    private readonly DatabaseService _db;

    public ScheduleRepository(DatabaseService db) => _db = db;

    public async Task UpsertAsync(string categoryId, List<RaceScheduleRecord> records)
    {
        var conn = await _db.GetConnectionAsync();
        var existing = await conn.Table<RaceScheduleRecord>()
            .Where(r => r.CategoryId == categoryId)
            .ToListAsync();

        foreach (var record in records)
        {
            var match = existing.FirstOrDefault(e => e.EventName == record.EventName);
            if (match is not null)
            {
                await conn.UpdateAsync(record);
            }
            else
            {
                await conn.InsertAsync(record);
            }
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
