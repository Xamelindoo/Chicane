using Chicane.Mobile.ViewModels;

namespace Chicane.Mobile.Views;

public partial class NewsPage : ContentPage
{
    public NewsPage(NewsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
