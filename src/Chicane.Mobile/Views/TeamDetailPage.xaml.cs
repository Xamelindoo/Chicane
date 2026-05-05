using Chicane.Mobile.ViewModels;

namespace Chicane.Mobile.Views;

public partial class TeamDetailPage : ContentPage
{
    public TeamDetailPage(TeamDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
