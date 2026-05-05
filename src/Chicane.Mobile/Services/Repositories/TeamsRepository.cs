using Chicane.Mobile.Models.Storage;

namespace Chicane.Mobile.Services.Repositories;

public class TeamsRepository
{
    private readonly DatabaseService _db;

    public TeamsRepository(DatabaseService db) => _db = db;

    public async Task UpsertAsync(string categoryId, List<TeamRecord> records)
    {
        var conn = await _db.GetConnectionAsync();
        var existing = await conn.Table<TeamRecord>()
            .Where(r => r.CategoryId == categoryId)
            .ToListAsync();

        foreach (var record in records)
        {
            var match = existing.FirstOrDefault(e => e.TeamName == record.TeamName);
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

    public async Task<List<TeamRecord>> GetByCategoryAsync(string categoryId)
    {
        var conn = await _db.GetConnectionAsync();
        return await conn.Table<TeamRecord>()
            .Where(r => r.CategoryId == categoryId)
            .OrderBy(r => r.Position)
            .ToListAsync();
    }
}
