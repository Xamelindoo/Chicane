using System.Collections.ObjectModel;
using Chicane.Mobile.Models;
using Chicane.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Graphics;

namespace Chicane.Mobile.ViewModels;

[QueryProperty(nameof(DriverNumber), "driverNumber")]
public partial class DriverDetailViewModel : ObservableObject
{
    private readonly OpenF1ApiService _openF1;

    public DriverDetailViewModel(OpenF1ApiService openF1)
    {
        _openF1 = openF1;
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
    private int driverNumber;

    [ObservableProperty]
    private string broadcastName = string.Empty;

    [ObservableProperty]
    private string fullName = string.Empty;

    [ObservableProperty]
    private string nameAcronym = string.Empty;

    [ObservableProperty]
    private string teamName = string.Empty;

    [ObservableProperty]
    private Color teamColour = Color.FromArgb("#333333");

    [ObservableProperty]
    private string headshotUrl = string.Empty;

    [ObservableProperty]
    private int totalPoints;

    [ObservableProperty]
    private int position;

    public ObservableCollection<DriverRaceResult> RaceResults { get; } = new();

    partial void OnDriverNumberChanged(int value)
    {
        HasData = false;
        if (value > 0 && !IsBusy)
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
            var detail = await _openF1.GetDriverDetailAsync(DriverNumber);
            if (detail is null)
            {
                IsEmptyState = true;
                return;
            }

            BroadcastName = detail.BroadcastName;
            FullName = detail.FullName;
            NameAcronym = detail.NameAcronym;
            TeamName = detail.TeamName;
            TeamColour = Color.FromArgb(detail.TeamColour is not null && !detail.TeamColour.StartsWith("#") ? "#" + detail.TeamColour : (detail.TeamColour ?? "333333"));
            HeadshotUrl = detail.HeadshotUrl;
            TotalPoints = detail.Points;
            Position = detail.Position;

            RaceResults.Clear();
            foreach (var r in detail.RaceResults)
                RaceResults.Add(r);

            HasData = true;
            IsEmptyState = detail.RaceResults.Count == 0;
        }
        catch (Exception)
        {
            ErrorMessage = "Unable to load driver data.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToTeamAsync()
    {
        if (string.IsNullOrWhiteSpace(TeamName)) return;
        await Shell.Current.GoToAsync($"team-detail?teamName={Uri.EscapeDataString(TeamName)}");
    }

    [RelayCommand]
    private async Task NavigateToRaceAsync(DriverRaceResult result)
    {
        if (result.SessionKey <= 0) return;
        await Shell.Current.GoToAsync($"past-race-detail?sessionKey={result.SessionKey}");
    }
}
