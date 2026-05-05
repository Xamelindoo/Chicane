using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public interface IRaceCalendarService
{
    Task<List<RaceScheduleEntry>> GetUpcomingRacesAsync();
    Task<List<RaceScheduleEntry>> GetPastRacesAsync();
    Task<PastRaceDetail> GetPastRaceDetailAsync(string eventName);
}
