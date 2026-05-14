using System.Collections.ObjectModel;
using Chicane.Mobile.Models;
using Chicane.Mobile.Services;
using Chicane.Mobile.Services.Repositories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chicane.Mobile.ViewModels;

/// <summary>
/// ViewModel for the Leaderboards tab (PILOTI / TEAM toggle).
/// Loads driver standings (with position arrows, headshots, team colors)
/// and team standings from cached services.
/// Supports navigation to driver and team detail pages.
/// Accepts a "tab" query parameter to pre-select PILOTI or TEAM.
/// </summary>
[QueryProperty(nameof(Tab), "tab")]
public partial class LeaderboardsViewModel : ObservableObject
{
    private readonly ITeamsService teamsService;
    private readonly StandingsRepository standingsRepo;

    public LeaderboardsViewModel(ITeamsService teamsService, StandingsRepository standingsRepo)
    {
        this.teamsService = teamsService;
        this.standingsRepo = standingsRepo;
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
    private bool isShowingPilots = true;

    [ObservableProperty]
    private bool isShowingTeams;

    private string? _tab;
    public string? Tab
    {
        set
        {
            _tab = value;
            if (value == "teams")
            {
                IsShowingTeams = true;
                IsShowingPilots = false;
            }
        }
    }

    public ObservableCollection<TeamEntry> Teams { get; } = new();
    public ObservableCollection<DriverStandingsEntry> Drivers { get; } = new();

    public List<int> AvailableYears => Enumerable.Range(2024, DateTime.UtcNow.Year - 2023).Reverse().ToList();

    [ObservableProperty]
    private int selectedYear = DateTime.UtcNow.Year;

    [ObservableProperty]
    private bool isRefreshing;

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            System.Diagnostics.Debug.WriteLine("[Leaderboards] Loading...");
            await LoadTeamsAsync();
            await LoadDriversAsync();
            System.Diagnostics.Debug.WriteLine($"[Leaderboards] Loaded: Teams={Teams.Count} Drivers={Drivers.Count}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Leaderboards] ERROR: {ex.Message}");
            ErrorMessage = "Unable to load leaderboards.";
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
        try
        {
            await standingsRepo.DeleteByCategoryAsync("f1");
            await LoadAsync();
        }
        finally { IsRefreshing = false; }
    }

    [RelayCommand]
    private void ShowTeams()
    {
        IsShowingTeams = true;
        IsShowingPilots = false;
    }

    [RelayCommand]
    private void ShowPilots()
    {
        IsShowingTeams = false;
        IsShowingPilots = true;
    }

    [RelayCommand]
    private async Task NavigateToDriverAsync(int driverNumber)
    {
        if (driverNumber <= 0) return;
        await Shell.Current.GoToAsync($"driver-detail?driverNumber={driverNumber}");
    }

    [RelayCommand]
    private async Task NavigateToDetailAsync(string teamName)
    {
        await Shell.Current.GoToAsync($"team-detail?teamName={Uri.EscapeDataString(teamName)}");
    }

    private async Task LoadTeamsAsync()
    {
        var teams = await teamsService.GetTeamsAsync("f1");
        Teams.Clear();
        foreach (var team in teams)
        {
            Teams.Add(team);
        }
        HasData = Teams.Count > 0 || Drivers.Count > 0;
    }

    private async Task LoadDriversAsync()
    {
        var drivers = await teamsService.GetDriversAsync("f1");
        Drivers.Clear();
        foreach (var driver in drivers)
        {
            Drivers.Add(driver);
        }
        HasData = Teams.Count > 0 || Drivers.Count > 0;
    }
}
