using Chicane.Mobile.ViewModels;

namespace Chicane.Mobile.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel viewModel;

    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;

        MainScrollView.Scrolled += OnScrolled;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!viewModel.HasData && !viewModel.IsBusy)
        {
            viewModel.LoadCommand.Execute(null);
        }
    }

    private void OnScrolled(object? sender, ScrolledEventArgs e)
    {
        viewModel.OnScrolled(e.ScrollY);
    }
}
