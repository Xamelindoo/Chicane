using Chicane.Mobile.ViewModels;

namespace Chicane.Mobile.Views;

public partial class PastRaceDetailPage : ContentPage
{
    public PastRaceDetailPage(PastRaceDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
