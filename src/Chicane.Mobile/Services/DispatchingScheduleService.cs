using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public class DispatchingScheduleService : IRaceCalendarService
{
    private readonly SportmonksService _sportmonks;
    private readonly MockRaceCalendarService _mock;

    public DispatchingScheduleService(SportmonksService sportmonks, MockRaceCalendarService mock)
    {
        _sportmonks = sportmonks;
        _mock = mock;
    }

    private IRaceCalendarService ResolveService(string categoryId) => categoryId switch
    {
        "f1" => _sportmonks,
        _ => _mock,
    };

    public Task<List<RaceScheduleEntry>> GetUpcomingRacesAsync(string categoryId)
        => ResolveService(categoryId).GetUpcomingRacesAsync(categoryId);

    public Task<List<RaceScheduleEntry>> GetPastRacesAsync(string categoryId)
        => ResolveService(categoryId).GetPastRacesAsync(categoryId);

    public Task<PastRaceDetail> GetPastRaceDetailAsync(string eventName, string categoryId)
        => ResolveService(categoryId).GetPastRaceDetailAsync(eventName, categoryId);
}
