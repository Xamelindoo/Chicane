using System.Collections.ObjectModel;
using Chicane.Mobile.Models;
using Chicane.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chicane.Mobile.ViewModels;

[QueryProperty(nameof(MeetingKey), "meetingKey")]
public partial class MeetingDetailViewModel : ObservableObject
{
    private readonly OpenF1ApiService _openF1;

    public MeetingDetailViewModel(OpenF1ApiService openF1)
    {
        _openF1 = openF1;
    }

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private int meetingKey;

    [ObservableProperty]
    private string meetingName = string.Empty;

    [ObservableProperty]
    private string circuitInfo = string.Empty;

    public ObservableCollection<SessionTime> Sessions { get; } = new();

    partial void OnMeetingKeyChanged(int value)
    {
        if (value > 0 && !IsBusy)
            LoadCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var schedule = await _openF1.GetMeetingScheduleAsync(false);
            var alsoPast = await _openF1.GetMeetingScheduleAsync(true);
            var all = schedule.Concat(alsoPast).ToList();
            var meeting = all.FirstOrDefault(m => m.MeetingKey == MeetingKey);
            if (meeting is null) return;

            MeetingName = meeting.MeetingName;
            CircuitInfo = $"{meeting.CircuitName}, {meeting.Country}";
            Sessions.Clear();
            foreach (var s in meeting.Sessions)
                Sessions.Add(s);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToSessionAsync(SessionTime session)
    {
        if (session.SessionKey <= 0) return;
        await Shell.Current.GoToAsync($"past-race-detail?sessionKey={session.SessionKey}");
    }
}
