using Chicane.Mobile.Services;

namespace Chicane.Mobile;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var version = Preferences.Get("db_schema_version", 0);
        System.Diagnostics.Debug.WriteLine($"[Cache] DB schema version: {version}");

        var db = IPlatformApplication.Current!.Services.GetService<DatabaseService>()!;
        _ = db.MigrateAsync();

        return new Window(new AppShell());
    }
}
