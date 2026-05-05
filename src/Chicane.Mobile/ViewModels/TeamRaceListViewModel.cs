using System.Collections.ObjectModel;
using Chicane.Mobile.Models;
using Chicane.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chicane.Mobile.ViewModels;

[QueryProperty(nameof(TeamName), "teamName")]
public partial class TeamRaceListViewModel : ObservableObject
{
    private readonly ITeamsService teamsService;

    public TeamRaceListViewModel(ITeamsService teamsService)
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
    private int totalPoints;

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
            var detail = await teamsService.GetTeamDetailAsync(TeamName);
            TotalPoints = detail.TotalPoints;

            RaceResults.Clear();
            foreach (var r in detail.RaceResults.OrderByDescending(r => r.EventDate))
            {
                RaceResults.Add(r);
            }

            HasData = RaceResults.Count > 0;
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

    [RelayCommand]
    private async Task NavigateToRaceAsync(string eventName)
    {
        await Shell.Current.GoToAsync($"past-race-detail?eventName={Uri.EscapeDataString(eventName)}");
    }
}
