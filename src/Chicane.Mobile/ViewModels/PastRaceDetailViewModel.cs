using System.Collections.ObjectModel;
using Chicane.Mobile.Models;
using Chicane.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chicane.Mobile.ViewModels;

[QueryProperty(nameof(SessionKey), "sessionKey")]
public partial class PastRaceDetailViewModel : ObservableObject
{
    private readonly IRaceCalendarService calendarService;

    public PastRaceDetailViewModel(IRaceCalendarService calendarService)
    {
        this.calendarService = calendarService;
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
    private int sessionKey;

    [ObservableProperty]
    private string eventDateText = string.Empty;

    public ObservableCollection<RaceResultEntry> Results { get; } = new();

    partial void OnSessionKeyChanged(int value)
    {
        HasData = false;
        if (value > 0 && !IsBusy)
        {
            LoadCommand.Execute(null);
        }
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            System.Diagnostics.Debug.WriteLine($"[Detail] Loading sessionKey={SessionKey}");
            var detail = await calendarService.GetPastRaceDetailAsync(SessionKey, "f1");
            System.Diagnostics.Debug.WriteLine($"[Detail] Results={detail.Results.Count} Drivers={detail.DriverStandings.Count} Teams={detail.ConstructorStandings.Count}");

            EventDateText = detail.EventDate.ToString("dd MMM yyyy");

            Results.Clear();
            var sorted = detail.Results
                .OrderBy(r => r.IsDnf ? 1 : 0)
                .ThenBy(r => r.IsDnf ? r.DriverName : null)
                .ThenBy(r => r.Position)
                .ToList();
            foreach (var r in sorted)
            {
                Results.Add(r);
            }

            HasData = true;
            IsEmptyState = Results.Count == 0;
        }
        catch (Exception)
        {
            ErrorMessage = "Unable to load race results (API may be rate-limited — try again in a moment).";
            HasData = false;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
