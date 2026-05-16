using Chicane.Mobile.ViewModels;

namespace Chicane.Mobile.Views;

public partial class MeetingDetailPage : ContentPage
{
    public MeetingDetailPage(MeetingDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
