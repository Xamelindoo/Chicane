using Chicane.Mobile.ViewModels;

namespace Chicane.Mobile.Views;

public partial class SchedulePage : ContentPage
{
    private readonly ScheduleViewModel viewModel;

    public SchedulePage(ScheduleViewModel viewModel)
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
