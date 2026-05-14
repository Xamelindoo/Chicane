using System.Collections.ObjectModel;
using Chicane.Mobile.Models;
using Chicane.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chicane.Mobile.ViewModels;

public partial class ScheduleViewModel : ObservableObject
{
    private readonly OpenF1ApiService _openF1;

    public ScheduleViewModel(OpenF1ApiService openF1)
    {
        _openF1 = openF1;
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
    private bool isShowingUpcoming = true;

    [ObservableProperty]
    private bool isShowingPast;

    public ObservableCollection<MeetingScheduleCard> Meetings { get; } = new();

    public List<int> AvailableYears => Enumerable.Range(2024, DateTime.UtcNow.Year - 2023).Reverse().ToList();

    [ObservableProperty]
    private int selectedYear = DateTime.UtcNow.Year;

    partial void OnSelectedYearChanged(int value)
    {
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
            await LoadMeetingsAsync();
        }
        catch (Exception)
        {
            ErrorMessage = "Unable to load schedule.";
            HasData = false;
            IsEmptyState = false;
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
    private async Task ShowUpcomingAsync()
    {
        IsShowingUpcoming = true;
        IsShowingPast = false;
        await LoadMeetingsAsync();
    }

    [RelayCommand]
    private async Task ShowPastAsync()
    {
        IsShowingUpcoming = false;
        IsShowingPast = true;
        await LoadMeetingsAsync();
    }

    [RelayCommand]
    private async Task NavigateToMeetingAsync(MeetingScheduleCard meeting)
    {
        await Shell.Current.GoToAsync($"meeting-detail?meetingKey={meeting.MeetingKey}");
    }

    private async Task LoadMeetingsAsync()
    {
        var meetings = await _openF1.GetMeetingScheduleAsync(!IsShowingUpcoming, SelectedYear);
        Meetings.Clear();
        foreach (var m in meetings)
            Meetings.Add(m);
        HasData = Meetings.Count > 0;
        IsEmptyState = Meetings.Count == 0;
    }
}
