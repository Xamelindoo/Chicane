using Chicane.Mobile.Models;
using Chicane.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace Chicane.Mobile.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly IF1DataService f1DataService;
    private IDispatcherTimer? countdownTimer;
    private DateTime eventDateTime;

    public HomeViewModel(IF1DataService f1DataService)
    {
        this.f1DataService = f1DataService;
    }

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string? errorMessage;

    [ObservableProperty]
    private bool hasData;

    [ObservableProperty]
    private bool isEmptyState;

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

    [ObservableProperty]
    private string weatherCondition = string.Empty;

    [ObservableProperty]
    private string weatherIcon = string.Empty;

    [ObservableProperty]
    private int weatherTemperature;

    [ObservableProperty]
    private string circuitName = string.Empty;

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
            var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
            HeroHeight = displayInfo.Height / displayInfo.Density;

            var raceEvent = await f1DataService.GetNextEventAsync();

            EventName = raceEvent.EventName;
            eventDateTime = raceEvent.ScheduledDateTime;
            CircuitName = raceEvent.Circuit.Name;
            CircuitImage = raceEvent.Circuit.ImageUrl;
            WeatherCondition = raceEvent.Weather.Condition;
            WeatherIcon = raceEvent.Weather.IconGlyph;
            WeatherTemperature = raceEvent.Weather.TemperatureC;

            var standings = await f1DataService.GetStandingsAsync();
            Standings.Clear();
            foreach (var entry in standings)
            {
                Standings.Add(entry);
            }

            var constructorStandings = await f1DataService.GetConstructorStandingsAsync();
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

    public void OnScrolled(double scrollY)
    {
        IsAtScrollTop = scrollY <= 0;
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
            return;
        }

        Countdown = $"{(int)remaining.TotalDays:D2}:{remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
    }
}
