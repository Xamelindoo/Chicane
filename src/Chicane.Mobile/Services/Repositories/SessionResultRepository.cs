using Chicane.Mobile.Models.Storage;

namespace Chicane.Mobile.Services.Repositories;

public class SessionResultRepository
{
    private readonly DatabaseService _db;

    public SessionResultRepository(DatabaseService db) => _db = db;

    public async Task<List<SessionResultRecord>> GetBySessionKeyAsync(int sessionKey)
    {
        var conn = await _db.GetConnectionAsync();
        return await conn.Table<SessionResultRecord>()
            .Where(r => r.SessionKey == sessionKey)
            .OrderBy(r => r.Position)
            .ToListAsync();
    }

    public async Task UpsertAsync(int sessionKey, List<SessionResultRecord> records)
    {
        var conn = await _db.GetConnectionAsync();
        await conn.ExecuteAsync("DELETE FROM SessionResults WHERE SessionKey = ?", sessionKey);
        foreach (var r in records)
            await conn.InsertAsync(r);
    }
}
