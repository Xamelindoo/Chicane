using Chicane.Mobile.Models;
using Chicane.Mobile.Models.Storage;
using Chicane.Mobile.Services.Repositories;

namespace Chicane.Mobile.Services;

/// <summary>
/// Cache-first wrapper around OpenF1ApiService for standings data.
/// All read methods check SQLite first; only when the cache is empty or stale
/// (missing HeadshotUrl for v2 migration) do they call the remote API.
/// API results are written to SQLite for subsequent instant access.
/// Headshot images are downloaded in background (fire-and-forget) to avoid blocking the UI.
/// </summary>
public class CachedF1DataService : IF1DataService
{
    private readonly IF1DataService _primary;
    private readonly StandingsRepository _standingsRepo;
    private readonly ConstructorStandingsRepository _constructorRepo;
    private readonly ImageCacheService _imgCache;

    public CachedF1DataService(
        IF1DataService primary,
        StandingsRepository standingsRepo,
        ConstructorStandingsRepository constructorRepo,
        ImageCacheService imgCache)
    {
        _primary = primary;
        _standingsRepo = standingsRepo;
        _constructorRepo = constructorRepo;
        _imgCache = imgCache;
    }

    // -------- Pass-through methods (not cached — dynamic data) --------

    public async Task<RaceEvent> GetNextEventAsync(string categoryId)
        => await _primary.GetNextEventAsync(categoryId);

    public async Task<LiveSessionData> GetLiveSessionDataAsync(string categoryId)
        => await _primary.GetLiveSessionDataAsync(categoryId);

    // -------- Driver Standings (cache-first, with image pre-download) --------

    /// <summary>
    /// Returns driver standings from SQLite cache if available and valid.
    /// Falls through to OpenF1 API on cache miss or stale cache (missing headshots).
    /// Headshot images are pre-downloaded in background via ImageCacheService.
    /// </summary>
    public async Task<List<StandingsEntry>> GetStandingsAsync(string categoryId)
    {
        // Check cache first — but skip if records have no HeadshotUrl (pre-v2 data)
        var cached = await _standingsRepo.GetByCategoryAsync(categoryId);
        if (cached.Count > 0 && !string.IsNullOrEmpty(cached[0].HeadshotUrl))
            return cached.Select(StorageMapper.ToDomain).ToList();

        // Cache miss or stale — fetch from remote API
        try
        {
            var standings = await _primary.GetStandingsAsync(categoryId);
            if (standings.Count > 0)
            {
                // Save to SQLite immediately (with remote URLs)
                var records = standings.Select(s => StorageMapper.ToRecord(s, categoryId)).ToList();
                await _standingsRepo.UpsertAsync(categoryId, records);

                // Background: download headshot images and update cache with local paths.
                // Each image download is tried independently — a single failure won't abort the batch.
                _ = Task.Run(async () =>
                {
                    try
                    {
                        foreach (var s in standings.Take(20))
                            s.HeadshotUrl = await _imgCache.CacheAsync(s.HeadshotUrl);
                        var updated = standings.Select(s => StorageMapper.ToRecord(s, categoryId)).ToList();
                        await _standingsRepo.UpsertAsync(categoryId, updated);
                    }
                    catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[ImgCache] Background standings failed: {ex.Message}"); }
                });
            }
            return standings;
        }
        catch
        {
            // API failed — return whatever cache we have (even if stale)
            return cached.Select(StorageMapper.ToDomain).ToList();
        }
    }

    // -------- Constructor Standings (cache-first) --------

    /// <summary>
    /// Returns constructor/team standings from SQLite cache if available.
    /// Falls through to OpenF1 API on cache miss.
    /// </summary>
    public async Task<List<ConstructorStandingsEntry>> GetConstructorStandingsAsync(string categoryId)
    {
        var cached = await _constructorRepo.GetByCategoryAsync(categoryId);
        if (cached.Count > 0)
            return cached.Select(StorageMapper.ToDomain).ToList();

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
            return cached.Select(StorageMapper.ToDomain).ToList();
        }
    }

    // -------- Track Positions (live data — not cached) --------

    public async Task<List<TrackPosition>> GetTrackPositionsAsync(string categoryId)
        => await _primary.GetTrackPositionsAsync(categoryId);
}
