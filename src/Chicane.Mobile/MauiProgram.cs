using Chicane.Mobile.Services;
using Chicane.Mobile.Services.Repositories;
using Chicane.Mobile.ViewModels;
using Chicane.Mobile.Views;
using Microsoft.Extensions.Logging;

namespace Chicane.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("SpaceMono-Regular.ttf", "SpaceMonoRegular");
                fonts.AddFont("SpaceMono-Bold.ttf", "SpaceMonoBold");
                fonts.AddFont("JetBrainsMono-Regular.ttf", "JetBrainsMonoRegular");
                fonts.AddFont("JetBrainsMono-Bold.ttf", "JetBrainsMonoBold");
                fonts.AddFont("BebasNeue-Regular.ttf", "BebasNeue");
                fonts.AddFont("Outfit-Regular.ttf", "OutfitRegular");
                fonts.AddFont("Outfit-Bold.ttf", "OutfitBold");
                fonts.AddFont("TitilliumWeb-Regular.ttf", "TitilliumWebRegular");
                fonts.AddFont("TitilliumWeb-SemiBold.ttf", "TitilliumWebSemiBold");
                fonts.AddFont("TitilliumWeb-Bold.ttf", "TitilliumWebBold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // API infrastructure
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<ErgastApiService>();
        builder.Services.AddSingleton<OpenF1Service>();
        builder.Services.AddSingleton<SportmonksService>();

        // News service
        builder.Services.AddSingleton<INewsService, Rss2JsonService>();
        builder.Services.AddSingleton<IWeatherService, OpenWeatherService>();

        // SQLite infrastructure
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<StandingsRepository>();
        builder.Services.AddSingleton<ConstructorStandingsRepository>();
        builder.Services.AddSingleton<ScheduleRepository>();
        builder.Services.AddSingleton<TeamsRepository>();

        // Mock services (dependencies for cached wrappers, fallback for non-F1)
        builder.Services.AddSingleton<MockF1DataService>();
        builder.Services.AddSingleton<MockTeamsService>();
        builder.Services.AddSingleton<MockRaceCalendarService>();

        // Data dispatching: F1 → real API, others → mock
        builder.Services.AddSingleton<DispatchingF1DataService>();

        // Cached services (real/mock → SQLite → return)
        builder.Services.AddSingleton<IF1DataService>(sp => new CachedF1DataService(
            sp.GetRequiredService<DispatchingF1DataService>(),
            sp.GetRequiredService<StandingsRepository>(),
            sp.GetRequiredService<ConstructorStandingsRepository>()));
        builder.Services.AddSingleton<IRaceCalendarService, CachedScheduleService>();
        builder.Services.AddSingleton<ITeamsService, CachedTeamsService>();
        builder.Services.AddSingleton<ICategoryProvider, CategoryProvider>();

        builder.Services.AddSingleton<CategorySwitcherViewModel>();
        builder.Services.AddTransient<CategorySwitcher>();

        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<HomePage>();

        builder.Services.AddTransient<ScheduleViewModel>();
        builder.Services.AddTransient<SchedulePage>();

        builder.Services.AddTransient<PastRaceDetailViewModel>();
        builder.Services.AddTransient<PastRaceDetailPage>();

        builder.Services.AddTransient<LeaderboardsViewModel>();
        builder.Services.AddTransient<LeaderboardsPage>();

        builder.Services.AddTransient<TeamDetailViewModel>();
        builder.Services.AddTransient<TeamDetailPage>();

        builder.Services.AddTransient<TeamRaceListViewModel>();
        builder.Services.AddTransient<TeamRaceListPage>();

        builder.Services.AddTransient<NewsViewModel>();
        builder.Services.AddTransient<NewsPage>();

        return builder.Build();
    }
}
