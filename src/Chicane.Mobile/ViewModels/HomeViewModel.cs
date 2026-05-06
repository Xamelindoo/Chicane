using Chicane.Mobile.Models;
using Chicane.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace Chicane.Mobile.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly IF1DataService f1DataService;
    private readonly ICategoryProvider categoryProvider;
    private readonly IWeatherService weatherService;
    private IDispatcherTimer? countdownTimer;
    private DateTime eventDateTime;

    public HomeViewModel(IF1DataService f1DataService, ICategoryProvider categoryProvider, IWeatherService weatherService)
    {
        this.f1DataService = f1DataService;
        this.categoryProvider = categoryProvider;
        this.weatherService = weatherService;

        categoryProvider.CategoryChanged += OnCategoryChanged;
    }

    private async void OnCategoryChanged(MotorsportCategory category)
    {
        if (HasData)
        {
            await LoadAsync();
        }
    }

    [ObservableProperty]
    private MotorsportCategory currentCategory = MotorsportCategory.F1;

    [ObservableProperty]
    private bool isBusy;

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
    private double heroHeight = 800;

    [ObservableProperty]
    private bool isAtScrollTop = true;

    [ObservableProperty]
    private bool isPreRace = true;

    [ObservableProperty]
    private bool isLive;

    [ObservableProperty]
    private string eventName = string.Empty;

    [ObservableProperty]
    private string countdown = "--:--:--:--";

    public ObservableCollection<CountdownPart> CountdownParts { get; } = new();

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
    private string circuitImage = string.Empty;

    [ObservableProperty]
    private string sessionName = string.Empty;

    [ObservableProperty]
    private string sessionStatus = string.Empty;

    [ObservableProperty]
    private int currentLap;

    [ObservableProperty]
    private int totalLaps;

    public ObservableCollection<DriverGap> PositionBoard { get; } = new();
    public ObservableCollection<StandingsEntry> Standings { get; } = new();
    public ObservableCollection<ConstructorStandingsEntry> ConstructorStandings { get; } = new();
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

            var categoryId = categoryProvider.CurrentCategory.Id;
            CurrentCategory = categoryProvider.CurrentCategory;

            var raceEvent = await f1DataService.GetNextEventAsync(categoryId);

            EventName = raceEvent.EventName;
            eventDateTime = raceEvent.ScheduledDateTime;
            CircuitName = raceEvent.Circuit.Name;
            CircuitLocation = $"{raceEvent.Circuit.Name}, {raceEvent.Circuit.Country}";
            CircuitMapUrl = GetCircuitMapUrl(raceEvent.Circuit.Country);
            HasCircuitMap = !string.IsNullOrWhiteSpace(CircuitMapUrl);
            CircuitImage = raceEvent.Circuit.ImageUrl;

            await LoadWeatherAsync(raceEvent);
            var standings = await f1DataService.GetStandingsAsync(categoryId);
            Standings.Clear();
            foreach (var entry in standings)
            {
                Standings.Add(entry);
            }

            var constructorStandings = await f1DataService.GetConstructorStandingsAsync(categoryId);
            ConstructorStandings.Clear();
            foreach (var entry in constructorStandings)
            {
                ConstructorStandings.Add(entry);
            }

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
                var liveData = await f1DataService.GetLiveSessionDataAsync(categoryProvider.CurrentCategory.Id);

                SessionName = liveData.SessionName;
                SessionStatus = liveData.Status;
                CurrentLap = liveData.CurrentLap;
                TotalLaps = liveData.TotalLaps;

                PositionBoard.Clear();
                foreach (var gap in liveData.PositionBoard)
                {
                    PositionBoard.Add(gap);
                }

                var trackPositions = await f1DataService.GetTrackPositionsAsync(categoryProvider.CurrentCategory.Id);
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
        else if (daysUntil <= 16)
        {
            WeatherForecast = new WeatherForecast
            {
                Condition = "Check back closer to race day",
                TemperatureC = 22,
                WindSpeed = 0,
                Humidity = 50,
                Pressure = 1013,
                IconGlyph = "\u26A0",
                IsApproximate = true,
            };
        }
        else
        {
            WeatherForecast = null;
        }
    }

    [RelayCommand]
    private void ToggleWeatherDetail()
    {
        if (WeatherForecast is null)
        {
            ErrorMessage = "Previsioni non disponibili";
            return;
        }
        if (WeatherForecast.IsApproximate)
        {
            ShowApproximateWarning = true;
            _ = Task.Run(async () => { await Task.Delay(2000); ShowApproximateWarning = false; });
        }
        ShowWeatherDetail = true;
    }

    [RelayCommand]
    private void CloseWeatherDetail()
    {
        ShowWeatherDetail = false;
    }

    private static string GetCircuitMapUrl(string country) => country switch
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

    private void StopCountdown()
    {
        if (countdownTimer is not null)
        {
            countdownTimer.Stop();
            countdownTimer.Tick -= OnCountdownTick;
            countdownTimer = null;
        }
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
            BuildCountdownParts("00:00:00:00");
            return;
        }

        var text = $"{(int)remaining.TotalDays:D2}:{remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
        Countdown = text;
        BuildCountdownParts(text);
    }

    private void BuildCountdownParts(string text)
    {
        CountdownParts.Clear();
        foreach (var ch in text)
        {
            CountdownParts.Add(new CountdownPart
            {
                Text = ch.ToString(),
                IsColon = ch == ':'
            });
        }
    }

    public void OnScrolled(double scrollY)
    {
        IsAtScrollTop = scrollY <= 0;
    }
}
