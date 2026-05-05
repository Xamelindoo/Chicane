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
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // SQLite infrastructure
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<StandingsRepository>();
        builder.Services.AddSingleton<ConstructorStandingsRepository>();
        builder.Services.AddSingleton<ScheduleRepository>();
        builder.Services.AddSingleton<TeamsRepository>();

        // Mock services (dependencies for cached wrappers)
        builder.Services.AddSingleton<MockF1DataService>();
        builder.Services.AddSingleton<MockTeamsService>();
        builder.Services.AddSingleton<MockRaceCalendarService>();

        // Cached services (mock → SQLite → return)
        builder.Services.AddSingleton<IF1DataService, CachedF1DataService>();
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
