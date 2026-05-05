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
        CategorySwitcherHost.Content = categorySwitcher;

        SetupCategoryPill();
        categorySwitcherViewModel.PropertyChanged += OnMenuStateChanged;
    }

    private void SetupCategoryPill()
    {
        var tap = new TapGestureRecognizer();
        tap.Tapped += (s, e) => categorySwitcherViewModel.ToggleMenuCommand.Execute(null);
        CategoryPill.GestureRecognizers.Add(tap);
        UpdatePillText();
    }

    private void UpdatePillText()
    {
        var arrow = categorySwitcherViewModel.IsMenuOpen ? "▲" : "▼";
        CategoryPill.FormattedText = new FormattedString
        {
            Spans =
            {
                new Span { Text = categorySwitcherViewModel.SelectedEmoji, FontSize = 14 },
                new Span { Text = " " },
                new Span { Text = categorySwitcherViewModel.SelectedName, FontFamily = "SpaceMonoBold", FontSize = 12, TextColor = Colors.White },
                new Span { Text = " " },
                new Span { Text = arrow, FontSize = 9, TextColor = Color.FromArgb("#A0A0A0") }
            }
        };
    }

    private void OnMenuStateChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CategorySwitcherViewModel.IsMenuOpen))
        {
            MainScrollView.IsEnabled = !categorySwitcherViewModel.IsMenuOpen;
            UpdatePillText();
        }
        if (e.PropertyName == nameof(CategorySwitcherViewModel.SelectedName) ||
            e.PropertyName == nameof(CategorySwitcherViewModel.SelectedEmoji))
        {
            UpdatePillText();
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
