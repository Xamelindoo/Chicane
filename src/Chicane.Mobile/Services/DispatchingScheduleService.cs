using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public class DispatchingScheduleService : IRaceCalendarService
{
    private readonly OpenF1ApiService _openF1;
    private readonly MockRaceCalendarService _mock;

    public DispatchingScheduleService(OpenF1ApiService openF1, MockRaceCalendarService mock)
    {
        _openF1 = openF1;
        _mock = mock;
    }

    private IRaceCalendarService ResolveService(string categoryId) => categoryId switch
    {
        "f1" => _openF1,
        _ => _mock,
    };

    public Task<List<RaceScheduleEntry>> GetUpcomingRacesAsync(string categoryId) => ResolveService(categoryId).GetUpcomingRacesAsync(categoryId);
    public Task<List<RaceScheduleEntry>> GetPastRacesAsync(string categoryId) => ResolveService(categoryId).GetPastRacesAsync(categoryId);
    public Task<PastRaceDetail> GetPastRaceDetailAsync(string eventName, string categoryId) => ResolveService(categoryId).GetPastRaceDetailAsync(eventName, categoryId);
}
