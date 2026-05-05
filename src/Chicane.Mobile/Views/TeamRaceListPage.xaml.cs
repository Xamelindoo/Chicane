using Chicane.Mobile.ViewModels;

namespace Chicane.Mobile.Views;

public partial class TeamRaceListPage : ContentPage
{
    public TeamRaceListPage(TeamRaceListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
