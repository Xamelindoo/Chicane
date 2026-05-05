using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public class DispatchingF1DataService : IF1DataService
{
    private readonly ErgastApiService _ergast;
    private readonly MockF1DataService _mock;

    public DispatchingF1DataService(ErgastApiService ergast, MockF1DataService mock)
    {
        _ergast = ergast;
        _mock = mock;
    }

    public Task<RaceEvent> GetNextEventAsync(string categoryId)
        => categoryId == "f1" ? _ergast.GetNextEventAsync(categoryId) : _mock.GetNextEventAsync(categoryId);

    public Task<LiveSessionData> GetLiveSessionDataAsync(string categoryId)
        => categoryId == "f1" ? _ergast.GetLiveSessionDataAsync(categoryId) : _mock.GetLiveSessionDataAsync(categoryId);

    public Task<List<StandingsEntry>> GetStandingsAsync(string categoryId)
        => categoryId == "f1" ? _ergast.GetStandingsAsync(categoryId) : _mock.GetStandingsAsync(categoryId);

    public Task<List<ConstructorStandingsEntry>> GetConstructorStandingsAsync(string categoryId)
        => categoryId == "f1" ? _ergast.GetConstructorStandingsAsync(categoryId) : _mock.GetConstructorStandingsAsync(categoryId);

    public Task<List<TrackPosition>> GetTrackPositionsAsync(string categoryId)
        => categoryId == "f1" ? _ergast.GetTrackPositionsAsync(categoryId) : _mock.GetTrackPositionsAsync(categoryId);
}
