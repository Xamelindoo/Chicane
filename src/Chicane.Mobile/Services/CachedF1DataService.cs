using Chicane.Mobile.Models;
using Chicane.Mobile.Models.Storage;
using Chicane.Mobile.Services.Repositories;

namespace Chicane.Mobile.Services;

public class CachedF1DataService : IF1DataService
{
    private readonly IF1DataService _primary;
    private readonly StandingsRepository _standingsRepo;
    private readonly ConstructorStandingsRepository _constructorRepo;

    public CachedF1DataService(
        IF1DataService primary,
        StandingsRepository standingsRepo,
        ConstructorStandingsRepository constructorRepo)
    {
        _primary = primary;
        _standingsRepo = standingsRepo;
        _constructorRepo = constructorRepo;
    }

    public async Task<RaceEvent> GetNextEventAsync(string categoryId)
        => await _primary.GetNextEventAsync(categoryId);

    public async Task<LiveSessionData> GetLiveSessionDataAsync(string categoryId)
        => await _primary.GetLiveSessionDataAsync(categoryId);

    public async Task<List<StandingsEntry>> GetStandingsAsync(string categoryId)
    {
        try
        {
            var standings = await _primary.GetStandingsAsync(categoryId);
            if (standings.Count > 0)
            {
                var records = standings.Select(s => StorageMapper.ToRecord(s, categoryId)).ToList();
                await _standingsRepo.UpsertAsync(categoryId, records);
            }
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
            var standings = await _primary.GetConstructorStandingsAsync(categoryId);
            if (standings.Count > 0)
            {
                var records = standings.Select(s => StorageMapper.ToRecord(s, categoryId)).ToList();
                await _constructorRepo.UpsertAsync(categoryId, records);
            }
            return standings;
        }
        catch
        {
            var records = await _constructorRepo.GetByCategoryAsync(categoryId);
            return records.Select(StorageMapper.ToDomain).ToList();
        }
    }

    public async Task<List<TrackPosition>> GetTrackPositionsAsync(string categoryId)
        => await _primary.GetTrackPositionsAsync(categoryId);
}
