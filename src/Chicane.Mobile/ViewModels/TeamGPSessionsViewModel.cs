using System.Collections.ObjectModel;
using Chicane.Mobile.Models;
using Chicane.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chicane.Mobile.ViewModels;

[QueryProperty(nameof(MeetingKey), "meetingKey")]
[QueryProperty(nameof(TeamName), "teamName")]
public partial class TeamGPSessionsViewModel : ObservableObject
{
    private readonly ITeamsService teamsService;
    private readonly OpenF1ApiService openF1;
    private List<TeamRaceResult> _allResults = new();

    public TeamGPSessionsViewModel(ITeamsService teamsService, OpenF1ApiService openF1)
    {
        this.teamsService = teamsService;
        this.openF1 = openF1;
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

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    [ObservableProperty]
    private int meetingKey;

    [ObservableProperty]
    private string teamName = string.Empty;

    [ObservableProperty]
    private string meetingName = string.Empty;

    [ObservableProperty]
    private int totalPoints;

    public ObservableCollection<TeamRaceResult> Sessions { get; } = new();

    partial void OnMeetingKeyChanged(int value)
    {
        if (value > 0 && !IsBusy && !string.IsNullOrWhiteSpace(TeamName))
            LoadCommand.Execute(null);
    }

    partial void OnTeamNameChanged(string value)
    {
        if (MeetingKey > 0 && !IsBusy && !string.IsNullOrWhiteSpace(value))
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
            var detailTask = teamsService.GetTeamDetailAsync(TeamName, "f1");
            var sessionsTask = openF1.GetSessionsByMeetingAsync(MeetingKey);

            await Task.WhenAll(detailTask, sessionsTask);

            var detail = detailTask.Result;
            _allResults = detail.RaceResults;
            var allSessions = sessionsTask.Result;

            // Meeting name from first session or team result
            var teamResults = _allResults.Where(r => r.MeetingKey == MeetingKey).ToList();
            var firstSession = allSessions.FirstOrDefault();
            MeetingName = teamResults.FirstOrDefault()?.EventName
                ?? firstSession?.SessionName ?? "GP Sessions";
            TotalPoints = teamResults.Sum(r => r.TotalPoints);

            Sessions.Clear();
            var teamDict = teamResults.ToDictionary(r => r.SessionKey);

            foreach (var s in allSessions.OrderBy(s => s.DateStart))
            {
                if (s.SessionKey <= 0) continue;
                var name = s.SessionName ?? s.SessionType ?? "Session";

                if (teamDict.TryGetValue(s.SessionKey, out var tr))
                {
                    Sessions.Add(tr);
                }
                else
                {
                    Sessions.Add(new TeamRaceResult
                    {
                        SessionName = name,
                        SessionKey = s.SessionKey,
                        MeetingKey = s.MeetingKey,
                        EventDate = s.DateStart,
                    });
                }
            }

            HasData = Sessions.Count > 0;
        }
        catch (Exception)
        {
            ErrorMessage = "Unable to load GP sessions.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToSessionAsync(TeamRaceResult session)
    {
        if (session.SessionKey <= 0) return;
        await Shell.Current.GoToAsync($"past-race-detail?sessionKey={session.SessionKey}");
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        try { await LoadAsync(); }
        finally { IsRefreshing = false; }
    }
}
