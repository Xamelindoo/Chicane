using Chicane.Mobile.Models.Storage;

namespace Chicane.Mobile.Services.Repositories;

public class StandingsRepository
{
    private readonly DatabaseService _db;

    public StandingsRepository(DatabaseService db) => _db = db;

    public async Task UpsertAsync(string categoryId, List<StandingsRecord> records)
    {
        var conn = await _db.GetConnectionAsync();
        var existing = await conn.Table<StandingsRecord>()
            .Where(r => r.CategoryId == categoryId)
            .ToListAsync();

        foreach (var record in records)
        {
            var match = existing.FirstOrDefault(e =>
                e.Position == record.Position &&
                e.CategoryId == record.CategoryId);

            if (match is not null)
            {
                record.Id = match.Id;
                await conn.UpdateAsync(record);
            }
            else
            {
                await conn.InsertAsync(record);
            }
        }
    }

    public async Task DeleteByCategoryAsync(string categoryId)
    {
        var conn = await _db.GetConnectionAsync();
        await conn.ExecuteAsync("DELETE FROM DriverStandings WHERE CategoryId = ?", categoryId);
    }

    public async Task<List<StandingsRecord>> GetByCategoryAsync(string categoryId)
    {
        var conn = await _db.GetConnectionAsync();
        return await conn.Table<StandingsRecord>()
            .Where(r => r.CategoryId == categoryId)
            .OrderBy(r => r.Position)
            .ToListAsync();
    }
}
