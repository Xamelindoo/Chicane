using Chicane.Mobile.ViewModels;

namespace Chicane.Mobile.Views;

public partial class DriverDetailPage : ContentPage
{
    public DriverDetailPage(DriverDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
