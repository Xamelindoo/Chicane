using Chicane.Mobile.ViewModels;

namespace Chicane.Mobile.Views;

public partial class TeamGPSessionsPage : ContentPage
{
    public TeamGPSessionsPage(TeamGPSessionsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
