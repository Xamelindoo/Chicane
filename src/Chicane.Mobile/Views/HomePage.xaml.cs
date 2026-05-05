using Chicane.Mobile.ViewModels;

namespace Chicane.Mobile.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel viewModel;
    private readonly CategorySwitcherViewModel categorySwitcherViewModel;

    public HomePage(HomeViewModel viewModel, CategorySwitcher categorySwitcher, CategorySwitcherViewModel categorySwitcherViewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
        this.categorySwitcherViewModel = categorySwitcherViewModel;

        MainScrollView.Scrolled += OnScrolled;
        CategorySwitcherHost.Children.Insert(0, categorySwitcher);

        categorySwitcherViewModel.PropertyChanged += OnMenuStateChanged;
    }

    private void OnMenuStateChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CategorySwitcherViewModel.IsMenuOpen))
        {
            MainScrollView.IsEnabled = !categorySwitcherViewModel.IsMenuOpen;
        }
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
