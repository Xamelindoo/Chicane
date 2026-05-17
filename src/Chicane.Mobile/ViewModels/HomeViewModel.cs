using Chicane.Mobile.Models;
using Chicane.Mobile.Services;
using Chicane.Mobile.Services.Repositories;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace Chicane.Mobile.ViewModels;

/// <summary>
/// Main view model for the Home tab.
/// Orchestrates loading of: next event countdown, GP weekend cards, weather,
/// driver/constructor standings previews, and top news.
/// Most data comes from cache-first services (instant after first load).
/// </summary>
public partial class HomeViewModel : ObservableObject
{
    private readonly IF1DataService f1DataService;
    private readonly IWeatherService weatherService;
    private readonly OpenF1ApiService openF1;
    private readonly CachedScheduleService cachedSchedule;
    private readonly INewsService newsService;
    private readonly StandingsRepository standingsRepo;
    private IDispatcherTimer? countdownTimer;
    private DateTime eventDateTime;

    public HomeViewModel(IF1DataService f1DataService, IWeatherService weatherService, OpenF1ApiService openF1, CachedScheduleService cachedSchedule, INewsService newsService, StandingsRepository standingsRepo)
    {
        this.f1DataService = f1DataService;
        this.weatherService = weatherService;
        this.openF1 = openF1;
        this.cachedSchedule = cachedSchedule;
        this.newsService = newsService;
        this.standingsRepo = standingsRepo;
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

    [ObservableProperty]
    private bool isEmptyState;

    [ObservableProperty]
    private bool isOffline;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    [ObservableProperty]
    private string eventName = string.Empty;

    [ObservableProperty]
    private string meetingName = string.Empty;

    [ObservableProperty]
    private string circuitImageUrl = string.Empty;

    [ObservableProperty]
    private double heroHeight = 800;

    [ObservableProperty]
    private bool isPreRace = true;

    [ObservableProperty]
    private bool isLive;

    [ObservableProperty]
    private string countdown = "--:--:--:--";

    public ObservableCollection<CountdownPart> CountdownParts { get; } = new();

    [ObservableProperty]
    private DateTime eventDate;

    [ObservableProperty]
    private WeatherForecast? weatherForecast;

    [ObservableProperty]
    private bool showWeatherDetail;

    [ObservableProperty]
    private bool showApproximateWarning;

    [ObservableProperty]
    private string circuitName = string.Empty;

    [ObservableProperty]
    private string circuitLocation = string.Empty;

    [ObservableProperty]
    private string circuitMapUrl = string.Empty;

    [ObservableProperty]
    private bool hasCircuitMap;

    [ObservableProperty]
    private double screenWidth;

    [ObservableProperty]
    private string circuitImage = string.Empty;

    [ObservableProperty]
    private string weatherIcon = string.Empty;

    [ObservableProperty]
    private string weatherCondition = string.Empty;

    [ObservableProperty]
    private double temperatureC;

    [ObservableProperty]
    private string sessionName = string.Empty;

    [ObservableProperty]
    private string sessionStatus = string.Empty;

    [ObservableProperty]
    private int currentLap;

    [ObservableProperty]
    private int totalLaps;

    [ObservableProperty]
    private int sessionKey;

    [ObservableProperty]
    private string liveLabel = "FOLLOW LIVE";

    public ObservableCollection<DriverGap> PositionBoard { get; } = new();
    public ObservableCollection<StandingsEntry> Standings { get; } = new();
    public ObservableCollection<ConstructorStandingsEntry> ConstructorStandings { get; } = new();
    public ObservableCollection<StandingsEntry> TopDrivers { get; } = new();
    public ObservableCollection<ConstructorStandingsEntry> TopTeams { get; } = new();
    public ObservableCollection<RaceWeekendCard> UpcomingWeekends { get; } = new();
    public ObservableCollection<NewsItem> TopNews { get; } = new();
    public ObservableCollection<TrackPosition> TrackPositions { get; } = new();

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            IsOffline = Connectivity.Current.NetworkAccess != NetworkAccess.Internet;

            var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
            HeroHeight = displayInfo.Height / displayInfo.Density;
            ScreenWidth = displayInfo.Width / displayInfo.Density - 36;

            var raceEvent = await f1DataService.GetNextEventAsync("f1");

            EventName = raceEvent.EventName;
            MeetingName = raceEvent.MeetingName;
            SessionKey = raceEvent.SessionKey;
            CircuitImageUrl = raceEvent.CircuitImageUrl;
            eventDateTime = raceEvent.ScheduledDateTime;
            EventDate = raceEvent.ScheduledDateTime;
            CircuitName = raceEvent.Circuit.Name;
            CircuitLocation = $"{raceEvent.Circuit.Name}, {raceEvent.Circuit.Location}";
            CircuitMapUrl = GetCircuitMapUrl(raceEvent.Circuit.Country);
            HasCircuitMap = !string.IsNullOrWhiteSpace(CircuitMapUrl);

            var weekendsTask = cachedSchedule.GetUpcomingWeekendsAsync(3);
            var newsTask = Task.Run(() => newsService.GetNewsAsync("f1"));
            var standingsTask = Task.Run(() => f1DataService.GetStandingsAsync("f1"));
            var ctorTask = Task.Run(() => f1DataService.GetConstructorStandingsAsync("f1"));
            var weatherTask = LoadWeatherAsync(raceEvent);

            await Task.WhenAll(weekendsTask, newsTask, standingsTask, ctorTask, weatherTask);

            UpcomingWeekends.Clear();
            foreach (var w in weekendsTask.Result)
                UpcomingWeekends.Add(w);

            if (UpcomingWeekends.Count > 0)
            {
                var first = UpcomingWeekends[0];
                var forecast = await weatherService.GetForecastAsync(first.CircuitName, first.Country, first.DateStart);
                if (forecast is not null)
                {
                    first.WeatherCondition = forecast.Condition;
                    first.WeatherIcon = forecast.IconGlyph;
                    first.TemperatureC = forecast.TemperatureC;
                    WeatherCondition = forecast.Condition;
                    WeatherIcon = forecast.IconGlyph;
                    TemperatureC = forecast.TemperatureC;
                }
                else
                {
                    first.WeatherCondition = "Check later";
                    first.WeatherIcon = "\u26A0";
                    WeatherCondition = "Check later";
                    WeatherIcon = "\u26A0";
                }
            }

            TopNews.Clear();
            foreach (var n in newsTask.Result.Take(4))
                TopNews.Add(n);
            CircuitImage = raceEvent.Circuit.ImageUrl;

            Standings.Clear();
            foreach (var entry in standingsTask.Result)
                Standings.Add(entry);

            ConstructorStandings.Clear();
            foreach (var entry in ctorTask.Result)
                ConstructorStandings.Add(entry);

            TopDrivers.Clear();
            foreach (var d in standingsTask.Result.Take(3))
                TopDrivers.Add(d);

            TopTeams.Clear();
            foreach (var t in ctorTask.Result.Take(3))
                TopTeams.Add(t);

            System.Diagnostics.Debug.WriteLine($"[Home] TopDrivers={TopDrivers.Count} TopTeams={TopTeams.Count}");

            IsPreRace = true;
            IsLive = false;
            HasData = true;
            IsEmptyState = false;

            StartCountdown();
        }
        catch (Exception)
        {
            ErrorMessage = "Unable to load race data.";
            HasData = false;
            IsEmptyState = false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        try
        {
            await standingsRepo.DeleteByCategoryAsync("f1");
            await LoadAsync();
        }
        finally { IsRefreshing = false; }
    }

    [RelayCommand]
    private async Task SimulateLiveAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            if (IsPreRace)
            {
                var liveData = await f1DataService.GetLiveSessionDataAsync("f1");

                SessionName = liveData.SessionName;
                SessionStatus = liveData.Status;
                CurrentLap = liveData.CurrentLap;
                TotalLaps = liveData.TotalLaps;

                PositionBoard.Clear();
                foreach (var gap in liveData.PositionBoard)
                {
                    PositionBoard.Add(gap);
                }

                var trackPositions = await f1DataService.GetTrackPositionsAsync("f1");
                TrackPositions.Clear();
                foreach (var tp in trackPositions)
                {
                    TrackPositions.Add(tp);
                }

                StopCountdown();
                IsPreRace = false;
                IsLive = true;
            }
            else
            {
                PositionBoard.Clear();
                TrackPositions.Clear();
                IsLive = false;
                IsPreRace = true;
                StartCountdown();
            }
        }
        catch (Exception)
        {
            ErrorMessage = "Unable to load live session data.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadWeatherAsync(RaceEvent raceEvent)
    {
        var daysUntil = (raceEvent.ScheduledDateTime - DateTime.UtcNow).TotalDays;

        if (daysUntil <= 7 && daysUntil > 0)
        {
            WeatherForecast = await weatherService.GetForecastAsync(
                raceEvent.Circuit.Name, raceEvent.Circuit.Country, raceEvent.ScheduledDateTime);
            if (WeatherForecast is not null)
                WeatherForecast = WeatherForecast with { IsApproximate = false };
        }
        else
        {
            WeatherForecast = new WeatherForecast
            {
                Condition = "Check back closer to race day",
                IsApproximate = true,
            };
        }
    }

    [RelayCommand]
    private void ToggleWeatherDetail()
    {
        if (WeatherForecast is null)
        {
            ErrorMessage = "Forecast not available";
            return;
        }
        if (WeatherForecast.IsApproximate)
        {
            ShowApproximateWarning = true;
            _ = Task.Run(async () =>
            {
                await Task.Delay(2000);
                MainThread.BeginInvokeOnMainThread(() => ShowApproximateWarning = false);
            });
            return;
        }
        ShowWeatherDetail = true;
    }

    [RelayCommand]
    private void CloseWeatherDetail()
    {
        ShowWeatherDetail = false;
    }

    [RelayCommand]
    private async Task NavigateToLiveAsync()
    {
        if (!IsLive) return;
        if (SessionKey <= 0) return;
        await Shell.Current.GoToAsync($"live?sessionKey={SessionKey}");
    }

    [RelayCommand]
    private async Task NavigateToDriverAsync(int driverNumber)
    {
        if (driverNumber <= 0) return;
        await Shell.Current.GoToAsync($"driver-detail?driverNumber={driverNumber}");
    }

    [RelayCommand]
    private async Task NavigateToLeaderboardsDriversAsync()
    {
        await Shell.Current.GoToAsync("//leaderboards?tab=drivers");
    }

    [RelayCommand]
    private async Task NavigateToLeaderboardsTeamsAsync()
    {
        await Shell.Current.GoToAsync("//leaderboards?tab=teams");
    }

    [RelayCommand]
    private async Task NavigateToScheduleAsync()
    {
        await Shell.Current.GoToAsync("//schedule");
    }

    [RelayCommand]
    private async Task NavigateToNewsAsync()
    {
        await Shell.Current.GoToAsync("//news");
    }

    private static string GetCircuitMapUrl(string country)
    {
        return country switch
        {
            "Australia" => "https://www.formula1.com/content/dam/fom-website/2018-redesign-assets/Circuit%20maps%2016x9/Australia_Circuit.png",
            "Italy" => "https://www.formula1.com/content/dam/fom-website/2018-redesign-assets/Circuit%20maps%2016x9/Italy_Circuit.png",
            "Great Britain" => "https://www.formula1.com/content/dam/fom-website/2018-redesign-assets/Circuit%20maps%2016x9/Great_Britain_Circuit.png",
            "Monaco" => "https://www.formula1.com/content/dam/fom-website/2018-redesign-assets/Circuit%20maps%2016x9/Monaco_Circuit.png",
            "Belgium" => "https://www.formula1.com/content/dam/fom-website/2018-redesign-assets/Circuit%20maps%2016x9/Belgium_Circuit.png",
            "Japan" => "https://www.formula1.com/content/dam/fom-website/2018-redesign-assets/Circuit%20maps%2016x9/Japan_Circuit.png",
            "USA" => "https://www.formula1.com/content/dam/fom-website/2018-redesign-assets/Circuit%20maps%2016x9/USA_Circuit.png",
            "Spain" => "https://www.formula1.com/content/dam/fom-website/2018-redesign-assets/Circuit%20maps%2016x9/Spain_Circuit.png",
            "France" => "https://www.formula1.com/content/dam/fom-website/2018-redesign-assets/Circuit%20maps%2016x9/France_Circuit.png",
            "Austria" => "https://www.formula1.com/content/dam/fom-website/2018-redesign-assets/Circuit%20maps%2016x9/Austria_Circuit.png",
            _ => "",
        };
    }

    private void StartCountdown()
    {
        StopCountdown();

        countdownTimer = Application.Current?.Dispatcher.CreateTimer();
        if (countdownTimer is null)
        {
            return;
        }

        countdownTimer.Interval = TimeSpan.FromSeconds(1);
        countdownTimer.Tick += OnCountdownTick;
        countdownTimer.Start();

        UpdateCountdown();
    }

    public void StopCountdown()
    {
        countdownTimer?.Stop();
    }

    public async Task RefreshWeatherIfStaleAsync()
    {
        if (WeatherCondition != "Check later") return;
        if (UpcomingWeekends.Count == 0) return;
        try
        {
            var first = UpcomingWeekends[0];
            var forecast = await weatherService.GetForecastAsync(first.CircuitName, first.Country, first.DateStart);
            if (forecast is not null)
            {
                first.WeatherCondition = forecast.Condition;
                first.WeatherIcon = forecast.IconGlyph;
                first.TemperatureC = forecast.TemperatureC;
                WeatherCondition = forecast.Condition;
                WeatherIcon = forecast.IconGlyph;
                TemperatureC = forecast.TemperatureC;
            }
        }
        catch { }
    }

    private void OnCountdownTick(object? sender, EventArgs e)
    {
        UpdateCountdown();
    }

    private void UpdateCountdown()
    {
        var remaining = eventDateTime - DateTime.UtcNow;

        if (remaining.TotalSeconds <= 0)
        {
            Countdown = "00:00:00:00";
            return;
        }

        var text = $"{(int)remaining.TotalDays:D2}:{remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
        Countdown = text;
    }

    [RelayCommand]
    private async Task OpenSettingsAsync()
    {
        await Shell.Current.GoToAsync("settings");
    }
}
