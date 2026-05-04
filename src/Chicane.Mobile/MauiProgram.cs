using Chicane.Mobile.Services;
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
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<IF1DataService, MockF1DataService>();

        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<HomePage>();

        builder.Services.AddTransient<StandingsViewModel>();
        builder.Services.AddTransient<StandingsPage>();

        builder.Services.AddTransient<TeamsViewModel>();
        builder.Services.AddTransient<TeamsPage>();

        builder.Services.AddTransient<NewsViewModel>();
        builder.Services.AddTransient<NewsPage>();

        return builder.Build();
    }
}
