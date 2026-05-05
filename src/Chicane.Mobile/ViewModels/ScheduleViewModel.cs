using System.Collections.ObjectModel;
using Chicane.Mobile.Models;
using Chicane.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chicane.Mobile.ViewModels;

public partial class ScheduleViewModel : ObservableObject
{
    private readonly IRaceCalendarService calendarService;
    private readonly ICategoryProvider categoryProvider;

    public ScheduleViewModel(IRaceCalendarService calendarService, ICategoryProvider categoryProvider)
    {
        this.calendarService = calendarService;
        this.categoryProvider = categoryProvider;

        categoryProvider.CategoryChanged += async _ => await LoadAsync();
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
    private bool isShowingUpcoming = true;

    [ObservableProperty]
    private bool isShowingPast;

    public ObservableCollection<RaceScheduleEntry> Races { get; } = new();

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            await LoadRacesAsync();
        }
        catch (Exception)
        {
            ErrorMessage = "Unable to load race schedule.";
            HasData = false;
            IsEmptyState = false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ShowUpcomingAsync()
    {
        IsShowingUpcoming = true;
        IsShowingPast = false;
        await LoadRacesAsync();
    }

    [RelayCommand]
    private async Task ShowPastAsync()
    {
        IsShowingUpcoming = false;
        IsShowingPast = true;
        await LoadRacesAsync();
    }

    [RelayCommand]
    private async Task NavigateToDetailAsync(RaceScheduleEntry entry)
    {
        if (!entry.IsCompleted)
            return;

        await Shell.Current.GoToAsync($"past-race-detail?eventName={Uri.EscapeDataString(entry.EventName)}");
    }

    private async Task LoadRacesAsync()
    {
        List<RaceScheduleEntry> races;

        if (IsShowingUpcoming)
        {
            races = await calendarService.GetUpcomingRacesAsync(categoryProvider.CurrentCategory.Id);
        }
        else
        {
            races = await calendarService.GetPastRacesAsync(categoryProvider.CurrentCategory.Id);
        }

        Races.Clear();
        foreach (var race in races)
        {
            Races.Add(race);
        }

        HasData = Races.Count > 0;
        IsEmptyState = Races.Count == 0;
    }
}
