using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chicane.Mobile.Services;

namespace Chicane.Mobile.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ICacheService cacheService;

    public SettingsViewModel(ICacheService cacheService)
    {
        this.cacheService = cacheService;
    }

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isClearing;

    [ObservableProperty]
    private double cacheSizeMB;

    [ObservableProperty]
    private double dbSizeMB;

    [ObservableProperty]
    private double imgSizeMB;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowClearedMessage))]
    private bool cacheCleared;

    public bool ShowClearedMessage => CacheCleared;

    [RelayCommand]
    private void Load()
    {
        IsBusy = true;
        var (total, db, img) = cacheService.GetCacheInfo();
        CacheSizeMB = total;
        DbSizeMB = db;
        ImgSizeMB = img;
        this.CacheCleared = false;
        IsBusy = false;
    }

    [RelayCommand]
    private async Task ClearCacheAsync()
    {
        IsClearing = true;
        await cacheService.ClearAllCacheAsync();
        var (total, db, img) = cacheService.GetCacheInfo();
        CacheSizeMB = total;
        DbSizeMB = db;
        ImgSizeMB = img;
        this.CacheCleared = true;
        IsClearing = false;
    }

    [RelayCommand]
    private async Task OpenPrivacyPolicyAsync()
    {
        await Browser.Default.OpenAsync("https://chicane.app/privacy", BrowserLaunchMode.SystemPreferred);
    }
}
