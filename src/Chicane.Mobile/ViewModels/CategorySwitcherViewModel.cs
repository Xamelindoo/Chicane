using System.Collections.ObjectModel;
using Chicane.Mobile.Models;
using Chicane.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chicane.Mobile.ViewModels;

public partial class CategorySwitcherViewModel : ObservableObject
{
    private readonly ICategoryProvider categoryProvider;

    public CategorySwitcherViewModel(ICategoryProvider categoryProvider)
    {
        this.categoryProvider = categoryProvider;
        SelectedId = categoryProvider.CurrentCategory.Id;
    }

    [ObservableProperty]
    private string selectedId = "f1";

    public ObservableCollection<MotorsportCategory> Categories { get; } = new(MotorsportCategory.All);

    [RelayCommand]
    private void SelectCategory(string categoryId)
    {
        var category = MotorsportCategory.All.FirstOrDefault(c => c.Id == categoryId);
        if (category is null) return;

        SelectedId = category.Id;
        categoryProvider.SetCategory(category);
    }
}
