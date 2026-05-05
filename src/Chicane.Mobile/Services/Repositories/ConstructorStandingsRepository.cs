using Chicane.Mobile.Models.Storage;

namespace Chicane.Mobile.Services.Repositories;

public class ConstructorStandingsRepository
{
    private readonly DatabaseService _db;

    public ConstructorStandingsRepository(DatabaseService db) => _db = db;

    public async Task UpsertAsync(string categoryId, List<ConstructorStandingsRecord> records)
    {
        var conn = await _db.GetConnectionAsync();
        var existing = await conn.Table<ConstructorStandingsRecord>()
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

    public async Task<List<ConstructorStandingsRecord>> GetByCategoryAsync(string categoryId)
    {
        var conn = await _db.GetConnectionAsync();
        return await conn.Table<ConstructorStandingsRecord>()
            .Where(r => r.CategoryId == categoryId)
            .OrderBy(r => r.Position)
            .ToListAsync();
    }
}
