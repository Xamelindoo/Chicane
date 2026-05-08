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
    private string gradientColor = "#333333";

    [ObservableProperty]
    private TeamSeasonStats? seasonStats;

    [ObservableProperty]
    private TeamProfile? profile;

    public ObservableCollection<DriverEntry> Drivers { get; } = new();
    public ObservableCollection<TeamRaceResult> RaceResults { get; } = new();

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
            GradientColor = detail.GradientColor;
            TotalPoints = detail.TotalPoints;
            SeasonStats = detail.SeasonStats;
            Profile = detail.Profile;

            Drivers.Clear();
            foreach (var d in detail.Drivers)
            {
                Drivers.Add(d);
            }

            RaceResults.Clear();
            foreach (var r in detail.RaceResults.OrderByDescending(r => r.EventDate))
            {
                RaceResults.Add(r);
            }

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
