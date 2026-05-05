using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public interface IRaceCalendarService
{
    Task<List<RaceScheduleEntry>> GetUpcomingRacesAsync(string categoryId);
    Task<List<RaceScheduleEntry>> GetPastRacesAsync(string categoryId);
    Task<PastRaceDetail> GetPastRaceDetailAsync(string eventName, string categoryId);
}
