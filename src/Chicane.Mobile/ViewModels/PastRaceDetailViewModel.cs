using System.Collections.ObjectModel;
using Chicane.Mobile.Models;
using Chicane.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chicane.Mobile.ViewModels;

[QueryProperty(nameof(EventName), "eventName")]
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
    private string eventName = string.Empty;

    [ObservableProperty]
    private int yellowFlagCount;

    public ObservableCollection<RaceResultEntry> Results { get; } = new();
    public ObservableCollection<StandingsEntry> DriverStandings { get; } = new();
    public ObservableCollection<ConstructorStandingsEntry> ConstructorStandings { get; } = new();

    partial void OnEventNameChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value) && !HasData && !IsBusy)
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
            var detail = await calendarService.GetPastRaceDetailAsync(EventName);

            YellowFlagCount = detail.YellowFlagCount;

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

            DriverStandings.Clear();
            foreach (var d in detail.DriverStandings)
            {
                DriverStandings.Add(d);
            }

            ConstructorStandings.Clear();
            foreach (var c in detail.ConstructorStandings)
            {
                ConstructorStandings.Add(c);
            }

            HasData = Results.Count > 0;
            IsEmptyState = Results.Count == 0;
        }
        catch (Exception)
        {
            ErrorMessage = "Unable to load race results.";
            HasData = false;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
