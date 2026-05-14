using Chicane.Mobile.ViewModels;

namespace Chicane.Mobile.Views;

public partial class LivePage : ContentPage
{
    private readonly LiveViewModel _viewModel;

    public LivePage(LiveViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.StopPolling();
    }
}
