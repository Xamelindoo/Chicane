using Chicane.Mobile.ViewModels;

namespace Chicane.Mobile.Views;

public partial class TeamsPage : ContentPage
{
    private readonly TeamsViewModel viewModel;

    public TeamsPage(TeamsViewModel viewModel)
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
