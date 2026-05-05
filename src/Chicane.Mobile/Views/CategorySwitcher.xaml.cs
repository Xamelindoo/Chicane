using Chicane.Mobile.ViewModels;

namespace Chicane.Mobile.Views;

public partial class CategorySwitcher : ContentView
{
    public CategorySwitcher(CategorySwitcherViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
