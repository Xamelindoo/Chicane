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
    public ObservableCollection<RaceGroupCard> RaceGroups { get; } = new();
    private List<TeamRaceResult> _allRaceResults = new();

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
            BuildRaceGroups();

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
    private async Task NavigateToGPSessionsAsync(RaceGroupCard? card)
    {
        if (card is null || card.MeetingKey <= 0) return;
        await Shell.Current.GoToAsync($"team-gp-sessions?meetingKey={card.MeetingKey}&teamName={Uri.EscapeDataString(TeamName)}");
    }

    private void BuildRaceGroups()
    {
        RaceGroups.Clear();
        var groups = _allRaceResults
            .GroupBy(r => r.EventName)
            .OrderByDescending(g => g.Max(r => r.EventDate));
        foreach (var g in groups)
        {
            var first = g.First();
            var d1Points = g.Sum(r => r.Driver1Points);
            var d2Points = g.Sum(r => r.Driver2Points);
            var raceSessions = g.Where(r => r.SessionName.Contains("Race", StringComparison.OrdinalIgnoreCase) && !r.SessionName.Contains("Sprint"));
            var mainResult = raceSessions.FirstOrDefault() ?? first;
            RaceGroups.Add(new RaceGroupCard
            {
                MeetingName = g.Key,
                MeetingKey = mainResult.MeetingKey,
                EventDate = first.EventDate,
                TotalPoints = g.Sum(r => r.TotalPoints),
                CircuitName = mainResult.CircuitName,
                Country = mainResult.Country,
                Driver1Name = first.Driver1Name,
                Driver1Code = first.Driver1Code,
                Driver1Points = d1Points,
                Driver1Position = mainResult.Driver1Position,
                Driver2Name = first.Driver2Name,
                Driver2Code = first.Driver2Code,
                Driver2Points = d2Points,
                Driver2Position = mainResult.Driver2Position,
            });
        }
    }
}
