using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public interface IF1DataService
{
    Task<RaceEvent> GetNextEventAsync(string categoryId);
    Task<LiveSessionData> GetLiveSessionDataAsync(string categoryId);
    Task<List<StandingsEntry>> GetStandingsAsync(string categoryId);
    Task<List<ConstructorStandingsEntry>> GetConstructorStandingsAsync(string categoryId);
    Task<List<TrackPosition>> GetTrackPositionsAsync(string categoryId);
}
