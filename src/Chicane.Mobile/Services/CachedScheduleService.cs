using Chicane.Mobile.Models;
using Chicane.Mobile.Models.Storage;
using Chicane.Mobile.Services.Repositories;

namespace Chicane.Mobile.Services;

/// <summary>
/// Cache-first wrapper around OpenF1ApiService for schedule and race data.
/// Provides SQLite-backed caching for:
///   - Race schedule (upcoming + past) → ScheduleRepository
///   - Past race results (session detail) → SessionResultRepository
///   - GP weekend cards (upcoming races on Home) → RaceWeekendRepository
/// All data that doesn't change (past results, season calendar) is never re-fetched
/// once cached. Images are downloaded in background to avoid blocking the UI.
/// </summary>
public class CachedScheduleService : IRaceCalendarService
{
    private readonly IRaceCalendarService _primary;
    private readonly OpenF1ApiService _openF1;
    private readonly ScheduleRepository _scheduleRepo;
    private readonly SessionResultRepository _sessionRepo;
    private readonly RaceWeekendRepository _weekendRepo;
    private readonly ImageCacheService _imgCache;

    public CachedScheduleService(
        IRaceCalendarService primary,
        OpenF1ApiService openF1,
        ScheduleRepository scheduleRepo,
        SessionResultRepository sessionRepo,
        RaceWeekendRepository weekendRepo,
        ImageCacheService imgCache)
    {
        _primary = primary;
        _openF1 = openF1;
        _scheduleRepo = scheduleRepo;
        _sessionRepo = sessionRepo;
        _weekendRepo = weekendRepo;
        _imgCache = imgCache;
    }

    // -------- Race Schedule (upcoming / past, cache-first) --------

    public async Task<List<RaceScheduleEntry>> GetUpcomingRacesAsync(string categoryId)
        => await CacheAndReturn($"{categoryId}_up", () => _primary.GetUpcomingRacesAsync(categoryId));

    public async Task<List<RaceScheduleEntry>> GetPastRacesAsync(string categoryId)
        => await CacheAndReturn($"{categoryId}_past", () => _primary.GetPastRacesAsync(categoryId));

    // -------- Past Race Detail (immutable — cache-first, never re-fetched) --------

    /// <summary>
    /// Returns the full race result for a given session (Practice, Qualifying, Sprint, Race).
    /// Once fetched and cached, the result is never requested from the API again.
    /// Headshot images are pre-downloaded in background.
    /// </summary>
    public async Task<PastRaceDetail> GetPastRaceDetailAsync(int sessionKey, string categoryId)
    {
        var cached = await _sessionRepo.GetBySessionKeyAsync(sessionKey);
        if (cached.Count > 0)
        {
            return new PastRaceDetail
            {
                EventDate = DateTime.UtcNow,
                Results = cached.Select(StorageMapper.ToDomain).ToList(),
            };
        }

        // Cache miss — fetch from API
        var detail = await _primary.GetPastRaceDetailAsync(sessionKey, categoryId);
        System.Diagnostics.Debug.WriteLine($"[CachedSchedule] sessionKey={sessionKey} results={detail.Results.Count}");

        if (detail.Results.Count > 0)
        {
            var records = detail.Results
                .Select(r => StorageMapper.ToRecord(r, sessionKey))
                .ToList();
            await _sessionRepo.UpsertAsync(sessionKey, records);

            // Background: download headshot images and re-save with local paths.
            // Each image is tried independently — a single failure won't abort the batch.
            _ = Task.Run(async () =>
            {
                try
                {
                    foreach (var r in detail.Results.Take(22))
                        r.HeadshotUrl = await _imgCache.CacheAsync(r.HeadshotUrl);
                    var updated = detail.Results
                        .Select(r => StorageMapper.ToRecord(r, sessionKey))
                        .ToList();
                    await _sessionRepo.UpsertAsync(sessionKey, updated);
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[ImgCache] Background results failed: {ex.Message}"); }
            });
        }
        return detail;
    }

    // -------- GP Weekend Cards (calendar — cache-first) --------

    /// <summary>
    /// Returns upcoming GP weekend cards for the Home page horizontal scroll.
    /// Always fetches fresh data — Redis on the backend handles shared caching (TTL 1h).
    /// Circuit images are downloaded live by the MAUI Image control.
    /// </summary>
    public async Task<List<RaceWeekendCard>> GetUpcomingWeekendsAsync(int count = 3)
    {
        return await _openF1.GetUpcomingWeekendsAsync(count);
    }

    // -------- Internal: generic cache-first helper for schedule lists --------

    /// <summary>
    /// Generic cache-first pattern: return cached data if available,
    /// otherwise fetch, cache, and return.
    /// </summary>
    private async Task<List<RaceScheduleEntry>> CacheAndReturn(
        string categoryId, Func<Task<List<RaceScheduleEntry>>> fetchFunc)
    {
        var cached = await _scheduleRepo.GetByCategoryAsync(categoryId);
        if (cached.Count > 0)
            return cached.Select(StorageMapper.ToDomain).ToList();

        try
        {
            var entries = await fetchFunc();
            var records = entries.Select(e => StorageMapper.ToRecord(e, categoryId)).ToList();
            await _scheduleRepo.UpsertAsync(categoryId, records);
            return entries;
        }
        catch
        {
            return cached.Select(StorageMapper.ToDomain).ToList();
        }
    }
}
