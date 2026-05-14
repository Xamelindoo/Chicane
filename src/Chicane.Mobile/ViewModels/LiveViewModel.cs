using System.Collections.ObjectModel;
using Chicane.Mobile.Models;
using Chicane.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chicane.Mobile.ViewModels;

/// <summary>
/// ViewModel for the live timing page.
/// Polls OpenF1 API every 3 seconds for positions, intervals, and lap count.
/// </summary>
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

    public ObservableCollection<DriverGap> Positions { get; } = new();

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
            var liveData = await _openF1.GetLiveDataAsync(SessionKey);
            SessionName = liveData.SessionName;
            MeetingName = liveData.MeetingName;
            CircuitName = liveData.CircuitName;
            CurrentLap = liveData.CurrentLap;
            TotalLaps = liveData.TotalLaps;
            LapsDisplay = $"{CurrentLap} / {TotalLaps}";

            Positions.Clear();
            foreach (var p in liveData.PositionBoard)
                Positions.Add(p);

            HasData = true;

            // Start polling every 3 seconds
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

    private async Task PollAsync()
    {
        try
        {
            var liveData = await _openF1.GetLiveDataAsync(SessionKey);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                CurrentLap = liveData.CurrentLap;
                TotalLaps = liveData.TotalLaps;
                LapsDisplay = $"{CurrentLap} / {TotalLaps}";

                Positions.Clear();
                foreach (var p in liveData.PositionBoard)
                    Positions.Add(p);
            });
        }
        catch { /* Silently skip failed polls */ }
    }

    public void StopPolling()
    {
        _pollTimer?.Stop();
    }
}
