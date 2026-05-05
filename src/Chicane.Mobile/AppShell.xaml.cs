using Chicane.Mobile.Views;

namespace Chicane.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("past-race-detail", typeof(PastRaceDetailPage));
        Routing.RegisterRoute("team-detail", typeof(TeamDetailPage));
        Routing.RegisterRoute("team-race-list", typeof(TeamRaceListPage));
    }
}
