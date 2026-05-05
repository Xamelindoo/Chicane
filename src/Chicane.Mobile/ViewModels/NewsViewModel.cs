using System.Collections.ObjectModel;
using Chicane.Mobile.Models;
using Chicane.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chicane.Mobile.ViewModels;

public partial class NewsViewModel : ObservableObject
{
    private readonly INewsService newsService;
    private readonly ICategoryProvider categoryProvider;

    public NewsViewModel(INewsService newsService, ICategoryProvider categoryProvider)
    {
        this.newsService = newsService;
        this.categoryProvider = categoryProvider;

        categoryProvider.CategoryChanged += async _ => await LoadAsync();
    }

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string? errorMessage;

    [ObservableProperty]
    private bool hasData;

    [ObservableProperty]
    private bool isEmptyState;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public ObservableCollection<NewsItem> Articles { get; } = new();

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var articles = await newsService.GetNewsAsync(categoryProvider.CurrentCategory.Id);

            Articles.Clear();
            foreach (var article in articles)
            {
                Articles.Add(article);
            }

            HasData = Articles.Count > 0;
            IsEmptyState = Articles.Count == 0;
        }
        catch
        {
            ErrorMessage = "Unable to load news.";
            HasData = false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task OpenArticleAsync(string url)
    {
        if (!string.IsNullOrWhiteSpace(url))
        {
            await Browser.Default.OpenAsync(new Uri(url), BrowserLaunchMode.SystemPreferred);
        }
    }
}
