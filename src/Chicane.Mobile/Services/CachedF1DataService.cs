using Chicane.Mobile.Models;
using Chicane.Mobile.Models.Storage;
using Chicane.Mobile.Services.Repositories;

namespace Chicane.Mobile.Services;

public class CachedF1DataService : IF1DataService
{
    private readonly MockF1DataService _mock;
    private readonly StandingsRepository _standingsRepo;
    private readonly ConstructorStandingsRepository _constructorRepo;
    private readonly ReadWriteLock _lock = new();

    public CachedF1DataService(
        MockF1DataService mock,
        StandingsRepository standingsRepo,
        ConstructorStandingsRepository constructorRepo)
    {
        _mock = mock;
        _standingsRepo = standingsRepo;
        _constructorRepo = constructorRepo;
    }

    public async Task<RaceEvent> GetNextEventAsync(string categoryId)
        => await _mock.GetNextEventAsync(categoryId);

    public async Task<LiveSessionData> GetLiveSessionDataAsync(string categoryId)
        => await _mock.GetLiveSessionDataAsync(categoryId);

    public async Task<List<StandingsEntry>> GetStandingsAsync(string categoryId)
    {
        try
        {
            var standings = await _mock.GetStandingsAsync(categoryId);
            var records = standings.Select(s => StorageMapper.ToRecord(s, categoryId)).ToList();
            await _standingsRepo.UpsertAsync(categoryId, records);
            return standings;
        }
        catch
        {
            var records = await _standingsRepo.GetByCategoryAsync(categoryId);
            return records.Select(StorageMapper.ToDomain).ToList();
        }
    }

    public async Task<List<ConstructorStandingsEntry>> GetConstructorStandingsAsync(string categoryId)
    {
        try
        {
            var standings = await _mock.GetConstructorStandingsAsync(categoryId);
            var records = standings.Select(s => StorageMapper.ToRecord(s, categoryId)).ToList();
            await _constructorRepo.UpsertAsync(categoryId, records);
            return standings;
        }
        catch
        {
            var records = await _constructorRepo.GetByCategoryAsync(categoryId);
            return records.Select(StorageMapper.ToDomain).ToList();
        }
    }

    public async Task<List<TrackPosition>> GetTrackPositionsAsync(string categoryId)
        => await _mock.GetTrackPositionsAsync(categoryId);

    private class ReadWriteLock { }
}
