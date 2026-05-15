using System.Collections.ObjectModel;
using Chicane.Mobile.Models;
using Chicane.Mobile.Models.Api;
using Chicane.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chicane.Mobile.ViewModels;

[QueryProperty(nameof(SessionKey), "sessionKey")]
public partial class LiveViewModel : ObservableObject
{
    private readonly OpenF1ApiService _openF1;
    private IDispatcherTimer? _pollTimer;

    public LiveViewModel(OpenF1ApiService openF1)
    {
        _openF1 = openF1;
    }

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string? errorMessage;

    [ObservableProperty]
    private bool hasData;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    [ObservableProperty]
    private int sessionKey;

    [ObservableProperty]
    private string sessionName = string.Empty;

    [ObservableProperty]
    private string meetingName = string.Empty;

    [ObservableProperty]
    private string circuitName = string.Empty;

    [ObservableProperty]
    private int currentLap;

    [ObservableProperty]
    private int totalLaps;

    [ObservableProperty]
    private string lapsDisplay = "— / —";

    [ObservableProperty]
    private string circuitImageUrl = string.Empty;

    [ObservableProperty]
    private TrackMapDrawable trackDrawable = new();

    [ObservableProperty]
    private bool showAllDrivers;

    public ObservableCollection<LiveDriverRow> Positions { get; } = new();
    public ObservableCollection<LiveCommentaryItem> Commentary { get; } = new();

    public IEnumerable<LiveDriverRow> VisiblePositions =>
        ShowAllDrivers ? Positions : Positions.Take(5);

    partial void OnShowAllDriversChanged(bool value) => OnPropertyChanged(nameof(VisiblePositions));

    partial void OnSessionKeyChanged(int value)
    {
        if (value > 0 && !IsBusy)
            LoadCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            await LoadSessionInfoAsync();
            await LoadLiveDataAsync();

            HasData = true;

            _pollTimer ??= Application.Current!.Dispatcher.CreateTimer();
            _pollTimer.Interval = TimeSpan.FromSeconds(3);
            _pollTimer.Tick += async (s, e) => await PollAsync();
            if (!_pollTimer.IsRunning)
                _pollTimer.Start();
        }
        catch (Exception)
        {
            ErrorMessage = "Unable to load live data.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadSessionInfoAsync()
    {
        var sessions = await _openF1.GetSessionsAsync(DateTime.UtcNow.Year);
        var session = sessions?.FirstOrDefault(s => s.SessionKey == SessionKey);
        await _openF1.GetDriversAsync(SessionKey);

        SessionName = session?.SessionName ?? "LIVE";
        MeetingName = session?.Location ?? "";
        CircuitName = session?.CircuitShortName ?? "";

        if (session?.MeetingKey > 0)
        {
            var img = await _openF1.GetMeetingCircuitImageAsync(session.MeetingKey, DateTime.UtcNow.Year);
            CircuitImageUrl = img ?? string.Empty;
        }
    }

    private async Task LoadLiveDataAsync()
    {
        var positionsTask = _openF1.GetPositionsAsync(SessionKey);
        var intervalsTask = _openF1.GetIntervalsAsync(SessionKey);
        var lapsTask = _openF1.GetLapsPerSessionAsync(SessionKey);
        var commentaryTask = _openF1.GetRaceControlAsync(SessionKey);
        var stintsTask = _openF1.GetStintsAsync(SessionKey);
        var locationsTask = _openF1.GetLocationsAsync(SessionKey);

        await Task.WhenAll(positionsTask, intervalsTask, lapsTask, commentaryTask, stintsTask, locationsTask);

        var positions = positionsTask.Result ?? new();
        var intervals = intervalsTask.Result;
        var laps = lapsTask.Result;
        var raceControl = commentaryTask.Result;
        var stints = stintsTask.Result;

        var maxLap = laps.Select(l => l.LapNumber).DefaultIfEmpty(0).Max();
        CurrentLap = maxLap;
        TotalLaps = 57;
        LapsDisplay = $"{CurrentLap} / {TotalLaps}";

        var intervalMap = intervals.ToDictionary(i => i.DriverNumber);

        // Latest lap per driver
        var lapMap = laps
            .GroupBy(l => l.DriverNumber)
            .ToDictionary(g => g.Key, g => g.MaxBy(l => l.LapNumber)?.LapDuration ?? 0);

        var stintMap = stints
            .GroupBy(s => s.DriverNumber)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(s => s.StintNumber).FirstOrDefault()?.Compound ?? "");

        var rows = new List<LiveDriverRow>();
        foreach (var p in positions.OrderBy(p => p.Position))
        {
            var code = _openF1.DriverCode(p.DriverNumber);
            intervalMap.TryGetValue(p.DriverNumber, out var interval);
            lapMap.TryGetValue(p.DriverNumber, out var lastLap);
            stintMap.TryGetValue(p.DriverNumber, out var compound);

            rows.Add(new LiveDriverRow
            {
                Position = p.Position,
                DriverNumber = p.DriverNumber,
                DriverCode = code,
                DriverDisplay = _openF1.DriverLastName(p.DriverNumber),
                TeamName = _openF1.DriverTeam(p.DriverNumber),
                TeamColour = "#" + (_openF1.DriverTeamColour(p.DriverNumber) ?? "333333"),
                HeadshotUrl = _openF1.DriverHeadshot(p.DriverNumber),
                GapDisplay = p.Position == 1 ? "Leader" : interval?.GapToLeader is { } g ? FormatGap(g) : "—",
                IntervalDisplay = interval?.Interval is { } i ? $"+{i:F1}s" : "",
                LastLapTime = lastLap > 0 ? $"{lastLap:F3}" : "",
                TyreCompound = compound ?? "",
            });
        }

        Positions.Clear();
        foreach (var r in rows) Positions.Add(r);

        // Track map dots
        var locations = locationsTask.Result ?? new();
        var dots = locations
            .Select(l =>
            {
                var colour = _openF1.DriverTeamColour(l.DriverNumber);
                return new LiveLocationDot
                {
                    Code = _openF1.DriverCode(l.DriverNumber),
                    Colour = $"#{colour}",
                    X = l.X,
                    Y = l.Y,
                };
            }).ToList();

        var drawable = new TrackMapDrawable { Locations = dots };
        MainThread.BeginInvokeOnMainThread(() => TrackDrawable = drawable);

        // Commentary
        Commentary.Clear();
        foreach (var rc in raceControl)
        {
            Commentary.Add(new LiveCommentaryItem
            {
                Icon = FlagIcon(rc.Flag ?? rc.Category),
                Message = rc.Message ?? rc.Category ?? "",
                LapInfo = rc.LapNumber > 0 ? $"Lap {rc.LapNumber}" : "",
                Date = rc.Date,
            });
        }
    }

    private async Task PollAsync()
    {
        try
        {
            await LoadLiveDataAsync();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                OnPropertyChanged(nameof(VisiblePositions));
            });
        }
        catch { }
    }

    [RelayCommand]
    private void ToggleDriver(LiveDriverRow? driver)
    {
        if (driver is null) return;
        foreach (var d in Positions)
        {
            if (d != driver) d.IsExpanded = false;
        }
        driver.IsExpanded = !driver.IsExpanded;

        if (driver.IsExpanded)
            _ = LoadTelemetryAsync(driver);
    }

    private async Task LoadTelemetryAsync(LiveDriverRow driver)
    {
        var carTask = _openF1.GetCarDataAsync(SessionKey, driver.DriverNumber);
        var pitTask = _openF1.GetPitStopsAsync(SessionKey);

        await Task.WhenAll(carTask, pitTask);

        var car = carTask.Result;
        if (car is not null)
        {
            driver.Speed = car.Speed;
            driver.Rpm = car.Rpm;
            driver.Gear = car.NGear;
            driver.Throttle = car.Throttle;
            driver.Brake = car.Brake;
            driver.DrsStatus = DrsLabel(car.Drs);
        }

        var latestPit = pitTask.Result
            .Where(p => p.DriverNumber == driver.DriverNumber)
            .MaxBy(p => p.Date);
        if (latestPit is not null)
            driver.LastPit = $"{latestPit.StopDuration:F1}s";
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        try { await LoadAsync(); }
        finally { IsRefreshing = false; }
    }

    [RelayCommand]
    private void ToggleShowAll()
    {
        ShowAllDrivers = !ShowAllDrivers;
    }

    public void StopPolling()
    {
        _pollTimer?.Stop();
    }

    private static string FormatGap(double seconds) =>
        seconds >= 60
            ? $"+{(int)(seconds / 60)}L"
            : $"+{seconds:F1}s";

    private static string DrsLabel(int drs) => drs switch
    {
        10 or 12 or 14 => "ON",
        8 => "Available",
        _ => "OFF"
    };

    private static string FlagIcon(string? flag) => flag switch
    {
        "YELLOW" => "\U0001f7e1",
        "DOUBLE YELLOW" => "\U0001f7e1\U0001f7e1",
        "GREEN" => "\U0001f7e2",
        "RED" => "\U0001f534",
        "CHEQUERED" => "\U0001f3c1",
        "BLUE" => "\U0001f535",
        "BLACK AND WHITE" => "\U0001f3f4",
        "SAFETY" => "\U0001f6a8",
        _ => "\u2139"
    };
}
