using System.Collections.ObjectModel;
using Chicane.Mobile.Models;
using Chicane.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chicane.Mobile.ViewModels;

public partial class LeaderboardsViewModel : ObservableObject
{
    private readonly ITeamsService teamsService;

    public LeaderboardsViewModel(ITeamsService teamsService)
    {
        this.teamsService = teamsService;
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
    private bool isShowingTeams = true;

    [ObservableProperty]
    private bool isShowingPilots;

    public ObservableCollection<TeamEntry> Teams { get; } = new();
    public ObservableCollection<DriverStandingsEntry> Drivers { get; } = new();

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            await LoadTeamsAsync();
            await LoadDriversAsync();
        }
        catch (Exception)
        {
            ErrorMessage = "Unable to load leaderboards.";
            HasData = false;
        }
        finally
        {
            IsBusy = false;
        }
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

    private async Task LoadTeamsAsync()
    {
        var teams = await teamsService.GetTeamsAsync();
        Teams.Clear();
        foreach (var team in teams)
        {
            Teams.Add(team);
        }
        HasData = Teams.Count > 0 || Drivers.Count > 0;
    }

    private async Task LoadDriversAsync()
    {
        var drivers = await teamsService.GetDriversAsync();
        Drivers.Clear();
        foreach (var driver in drivers)
        {
            Drivers.Add(driver);
        }
        HasData = Teams.Count > 0 || Drivers.Count > 0;
    }
}
