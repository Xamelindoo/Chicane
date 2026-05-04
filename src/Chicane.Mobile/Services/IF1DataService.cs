using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public interface IF1DataService
{
    Task<RaceEvent> GetNextEventAsync();
    Task<LiveSessionData> GetLiveSessionDataAsync();
    Task<List<StandingsEntry>> GetStandingsAsync();
    Task<List<TrackPosition>> GetTrackPositionsAsync();
}
