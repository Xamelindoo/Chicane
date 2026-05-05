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
        UpdateSelectedVisual();
    }

    [ObservableProperty]
    private string selectedId = "f1";

    [ObservableProperty]
    private string selectedName = "Formula 1";

    [ObservableProperty]
    private string selectedEmoji = "🏎️";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ArrowIndicator))]
    private bool isMenuOpen;

    public string ArrowIndicator => IsMenuOpen ? "▲" : "▼";

    public ObservableCollection<MotorsportCategory> Categories { get; } = new(MotorsportCategory.All);

    [RelayCommand]
    private void ToggleMenu()
    {
        IsMenuOpen = !IsMenuOpen;
    }

    [RelayCommand]
    private void CloseMenu()
    {
        IsMenuOpen = false;
    }

    [RelayCommand]
    private void SelectCategory(string categoryId)
    {
        var category = MotorsportCategory.All.FirstOrDefault(c => c.Id == categoryId);
        if (category is null) return;

        SelectedId = category.Id;
        SelectedName = category.Name;
        SelectedEmoji = category.Emoji;
        IsMenuOpen = false;
        categoryProvider.SetCategory(category);
    }

    private void UpdateSelectedVisual()
    {
        var current = categoryProvider.CurrentCategory;
        SelectedName = current.Name;
        SelectedEmoji = current.Emoji;
    }
}
