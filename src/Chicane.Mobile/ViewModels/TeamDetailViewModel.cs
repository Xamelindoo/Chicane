using System.Collections.ObjectModel;
using Chicane.Mobile.Models;
using Chicane.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chicane.Mobile.ViewModels;

[QueryProperty(nameof(TeamName), "teamName")]
public partial class TeamDetailViewModel : ObservableObject
{
    private readonly ITeamsService teamsService;

    public TeamDetailViewModel(ITeamsService teamsService)
    {
        this.teamsService = teamsService;
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

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    [ObservableProperty]
    private string teamName = string.Empty;

    [ObservableProperty]
    private string borderColor = "#333333";

    [ObservableProperty]
    private int totalPoints;

    [ObservableProperty]
    private string emoji = "🏎️";

    [ObservableProperty]
    private string teamLogo = string.Empty;

    [ObservableProperty]
    private string teamLogoTransparent = string.Empty;

    [ObservableProperty]
    private string gradientColor = "#333333";

    [ObservableProperty]
    private TeamSeasonStats? seasonStats;

    [ObservableProperty]
    private TeamProfile? profile;

    public ObservableCollection<DriverEntry> Drivers { get; } = new();
    public ObservableCollection<TeamRaceResult> RaceResults { get; } = new();
    private List<TeamRaceResult> _allRaceResults = new();

    public List<string> AvailableFilters { get; } = new() { "ALL", "RACE", "QUALIFYING", "SPRINT", "SPRINT QUALI", "PRACTICE" };

    [ObservableProperty]
    private string selectedFilter = "ALL";

    [ObservableProperty]
    private bool isFilterExpanded;

    partial void OnSelectedFilterChanged(string value)
    {
        ApplyFilter();
    }

    [RelayCommand]
    private void ToggleFilter()
    {
        IsFilterExpanded = !IsFilterExpanded;
    }

    [RelayCommand]
    private void SelectFilter(string filter)
    {
        SelectedFilter = filter;
        IsFilterExpanded = false;
    }

    private void ApplyFilter()
    {
        var filtered = _allRaceResults.AsEnumerable();
        if (SelectedFilter != "ALL")
        {
            filtered = SelectedFilter switch
            {
                "RACE" => _allRaceResults.Where(r => r.SessionName.Contains("Race", StringComparison.OrdinalIgnoreCase) && !r.SessionName.Contains("Sprint")),
                "QUALIFYING" => _allRaceResults.Where(r => r.SessionName.Contains("Qualifying", StringComparison.OrdinalIgnoreCase) && !r.SessionName.Contains("Sprint")),
                "SPRINT" => _allRaceResults.Where(r => r.SessionName.Contains("Sprint", StringComparison.OrdinalIgnoreCase) && r.SessionName.Contains("Race") && !r.SessionName.Contains("Quali")),
                "SPRINT QUALI" => _allRaceResults.Where(r => r.SessionName.Contains("Sprint", StringComparison.OrdinalIgnoreCase) && r.SessionName.Contains("Quali")),
                "PRACTICE" => _allRaceResults.Where(r => r.SessionName.Contains("Practice", StringComparison.OrdinalIgnoreCase)),
                _ => _allRaceResults
            };
        }
        RaceResults.Clear();
        foreach (var r in filtered.OrderByDescending(r => r.EventDate))
            RaceResults.Add(r);
    }

    partial void OnTeamNameChanged(string value)
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
            var detail = await teamsService.GetTeamDetailAsync(TeamName, "f1");

            TeamName = detail.TeamName;
            BorderColor = detail.BorderColor;
            Emoji = detail.Emoji;
            TeamLogo = detail.TeamLogo;
            TeamLogoTransparent = detail.TeamLogoTransparent;
            GradientColor = detail.GradientColor;
            TotalPoints = detail.TotalPoints;
            SeasonStats = detail.SeasonStats;
            Profile = detail.Profile;

            Drivers.Clear();
            foreach (var d in detail.Drivers)
            {
                Drivers.Add(d);
            }

            _allRaceResults = detail.RaceResults.OrderByDescending(r => r.EventDate).ToList();
            ApplyFilter();

            HasData = true;
        }
        catch (Exception)
        {
            ErrorMessage = "Unable to load team details.";
            HasData = false;
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
        try { await LoadAsync(); }
        finally { IsRefreshing = false; }
    }

    [RelayCommand]
    private async Task NavigateToRaceAsync(string eventName)
    {
        await Shell.Current.GoToAsync($"past-race-detail?sessionKey=0");
    }

    [RelayCommand]
    private async Task NavigateToRaceListAsync()
    {
        await Shell.Current.GoToAsync($"team-race-list?teamName={Uri.EscapeDataString(TeamName)}");
    }
}
