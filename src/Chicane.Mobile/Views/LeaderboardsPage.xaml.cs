using Chicane.Mobile.ViewModels;

namespace Chicane.Mobile.Views;

public partial class LeaderboardsPage : ContentPage
{
    private readonly LeaderboardsViewModel viewModel;

    public LeaderboardsPage(LeaderboardsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!viewModel.HasData && !viewModel.IsBusy)
        {
            viewModel.LoadCommand.Execute(null);
        }
    }
}
