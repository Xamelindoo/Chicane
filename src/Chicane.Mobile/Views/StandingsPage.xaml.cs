using Chicane.Mobile.ViewModels;

namespace Chicane.Mobile.Views;

public partial class StandingsPage : ContentPage
{
    public StandingsPage(StandingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
