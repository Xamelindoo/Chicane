using Chicane.Mobile.ViewModels;

namespace Chicane.Mobile.Views;

public partial class TeamsPage : ContentPage
{
    public TeamsPage(TeamsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
