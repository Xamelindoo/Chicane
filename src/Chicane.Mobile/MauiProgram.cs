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
        builder.Services.AddSingleton<OpenF1TokenService>();
        builder.Services.AddSingleton<OpenF1ApiService>();

        // News service
        builder.Services.AddSingleton<INewsService, Rss2JsonService>();
        builder.Services.AddSingleton<IWeatherService, OpenWeatherService>();

        // SQLite infrastructure
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<StandingsRepository>();
        builder.Services.AddSingleton<ConstructorStandingsRepository>();
        builder.Services.AddSingleton<ScheduleRepository>();
        builder.Services.AddSingleton<TeamsRepository>();

        // Mock services (fallback only)
        builder.Services.AddSingleton<MockTeamsService>();
        builder.Services.AddSingleton<MockRaceCalendarService>();

        // Cached services (OpenF1 → SQLite → return)
        builder.Services.AddSingleton<IF1DataService>(sp => new CachedF1DataService(
            sp.GetRequiredService<OpenF1ApiService>(),
            sp.GetRequiredService<StandingsRepository>(),
            sp.GetRequiredService<ConstructorStandingsRepository>()));
        builder.Services.AddSingleton<IRaceCalendarService>(sp => new CachedScheduleService(
            sp.GetRequiredService<OpenF1ApiService>(),
            sp.GetRequiredService<ScheduleRepository>()));
        builder.Services.AddSingleton<ITeamsService, CachedTeamsService>();

        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<HomePage>();

        builder.Services.AddTransient<ScheduleViewModel>();
        builder.Services.AddTransient<SchedulePage>();

        builder.Services.AddTransient<MeetingDetailViewModel>();
        builder.Services.AddTransient<MeetingDetailPage>();

        builder.Services.AddTransient<PastRaceDetailViewModel>();
        builder.Services.AddTransient<PastRaceDetailPage>();

        builder.Services.AddTransient<DriverDetailViewModel>();
        builder.Services.AddTransient<DriverDetailPage>();

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
