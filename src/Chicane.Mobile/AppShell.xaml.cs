using Chicane.Mobile.Views;

namespace Chicane.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("meeting-detail", typeof(MeetingDetailPage));
        Routing.RegisterRoute("past-race-detail", typeof(PastRaceDetailPage));
        Routing.RegisterRoute("team-detail", typeof(TeamDetailPage));
        Routing.RegisterRoute("team-race-list", typeof(TeamRaceListPage));
        Routing.RegisterRoute("team-gp-sessions", typeof(TeamGPSessionsPage));
        Routing.RegisterRoute("driver-detail", typeof(DriverDetailPage));
        Routing.RegisterRoute("live", typeof(LivePage));
        Routing.RegisterRoute("settings", typeof(SettingsPage));
    }
}
