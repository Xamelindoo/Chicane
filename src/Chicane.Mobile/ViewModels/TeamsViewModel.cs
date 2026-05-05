using System.Collections.ObjectModel;
using Chicane.Mobile.Models;
using Chicane.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chicane.Mobile.ViewModels;

public partial class TeamsViewModel : ObservableObject
{
    private readonly ITeamsService teamsService;

    public TeamsViewModel(ITeamsService teamsService)
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

    public ObservableCollection<TeamEntry> Teams { get; } = new();

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var teams = await teamsService.GetTeamsAsync();

            Teams.Clear();
            foreach (var team in teams)
            {
                Teams.Add(team);
            }

            HasData = Teams.Count > 0;
            IsEmptyState = Teams.Count == 0;
        }
        catch (Exception)
        {
            ErrorMessage = "Unable to load team data.";
            HasData = false;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
