using System.Collections.ObjectModel;
using Chicane.Mobile.Models;
using Chicane.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chicane.Mobile.ViewModels;

public partial class LeaderboardsViewModel : ObservableObject
{
    private readonly ITeamsService teamsService;
    private readonly ICategoryProvider categoryProvider;

    public LeaderboardsViewModel(ITeamsService teamsService, ICategoryProvider categoryProvider)
    {
        this.teamsService = teamsService;
        this.categoryProvider = categoryProvider;
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

    [RelayCommand]
    private async Task NavigateToDetailAsync(string teamName)
    {
        await Shell.Current.GoToAsync($"team-detail?teamName={Uri.EscapeDataString(teamName)}");
    }

    private async Task LoadTeamsAsync()
    {
        var teams = await teamsService.GetTeamsAsync(categoryProvider.CurrentCategory.Id);
        Teams.Clear();
        foreach (var team in teams)
        {
            Teams.Add(team);
        }
        HasData = Teams.Count > 0 || Drivers.Count > 0;
    }

    private async Task LoadDriversAsync()
    {
        var drivers = await teamsService.GetDriversAsync(categoryProvider.CurrentCategory.Id);
        Drivers.Clear();
        foreach (var driver in drivers)
        {
            Drivers.Add(driver);
        }
        HasData = Teams.Count > 0 || Drivers.Count > 0;
    }
}
