using Chicane.Mobile.ViewModels;

namespace Chicane.Mobile.Views;

public partial class NewsPage : ContentPage
{
    private readonly NewsViewModel viewModel;

    public NewsPage(NewsViewModel viewModel)
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
