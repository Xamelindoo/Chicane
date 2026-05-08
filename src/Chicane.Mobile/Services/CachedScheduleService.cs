using Chicane.Mobile.Models;
using Chicane.Mobile.Models.Storage;
using Chicane.Mobile.Services.Repositories;

namespace Chicane.Mobile.Services;

public class CachedScheduleService : IRaceCalendarService
{
    private readonly IRaceCalendarService _primary;
    private readonly ScheduleRepository _scheduleRepo;

    public CachedScheduleService(IRaceCalendarService primary, ScheduleRepository scheduleRepo)
    {
        _primary = primary;
        _scheduleRepo = scheduleRepo;
    }

    public async Task<List<RaceScheduleEntry>> GetUpcomingRacesAsync(string categoryId)
        => await CacheAndReturn(categoryId, () => _primary.GetUpcomingRacesAsync(categoryId));

    public async Task<List<RaceScheduleEntry>> GetPastRacesAsync(string categoryId)
        => await CacheAndReturn(categoryId, () => _primary.GetPastRacesAsync(categoryId));

    public async Task<PastRaceDetail> GetPastRaceDetailAsync(int sessionKey, string categoryId)
        => await _primary.GetPastRaceDetailAsync(sessionKey, categoryId);

    private async Task<List<RaceScheduleEntry>> CacheAndReturn(
        string categoryId, Func<Task<List<RaceScheduleEntry>>> fetchFunc)
    {
        try
        {
            var entries = await fetchFunc();
            var records = entries.Select(e => StorageMapper.ToRecord(e, categoryId)).ToList();
            await _scheduleRepo.UpsertAsync(categoryId, records);
            return entries;
        }
        catch
        {
            var records = await _scheduleRepo.GetByCategoryAsync(categoryId);
            return records.Select(StorageMapper.ToDomain).ToList();
        }
    }
}
