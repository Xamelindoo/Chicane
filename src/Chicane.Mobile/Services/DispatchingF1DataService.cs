using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public class DispatchingF1DataService : IF1DataService
{
    private readonly ErgastApiService _ergast;
    private readonly SportmonksService _sportmonks;
    private readonly MockF1DataService _mock;

    public DispatchingF1DataService(ErgastApiService ergast, SportmonksService sportmonks, MockF1DataService mock)
    {
        _ergast = ergast;
        _sportmonks = sportmonks;
        _mock = mock;
    }

    private IF1DataService ResolveService(string categoryId) => categoryId switch
    {
        "f1" => _ergast,
        "motogp" => _sportmonks,
        _ => _mock,
    };

    public Task<RaceEvent> GetNextEventAsync(string categoryId)
        => ResolveService(categoryId).GetNextEventAsync(categoryId);

    public Task<LiveSessionData> GetLiveSessionDataAsync(string categoryId)
        => ResolveService(categoryId).GetLiveSessionDataAsync(categoryId);

    public Task<List<StandingsEntry>> GetStandingsAsync(string categoryId)
        => ResolveService(categoryId).GetStandingsAsync(categoryId);

    public Task<List<ConstructorStandingsEntry>> GetConstructorStandingsAsync(string categoryId)
        => ResolveService(categoryId).GetConstructorStandingsAsync(categoryId);

    public Task<List<TrackPosition>> GetTrackPositionsAsync(string categoryId)
        => ResolveService(categoryId).GetTrackPositionsAsync(categoryId);
}
