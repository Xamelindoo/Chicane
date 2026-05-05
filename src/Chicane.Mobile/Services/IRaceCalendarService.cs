using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public interface IRaceCalendarService
{
    Task<List<RaceScheduleEntry>> GetUpcomingRacesAsync();
    Task<List<RaceScheduleEntry>> GetPastRacesAsync();
}
