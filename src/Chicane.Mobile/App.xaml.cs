namespace Chicane.Mobile;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var isFirstLaunch = Preferences.Get("db_initialized_v1", false) == false;
        if (isFirstLaunch)
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "chicane.db");
            try
            {
                if (File.Exists(dbPath))
                    File.Delete(dbPath);
                System.Diagnostics.Debug.WriteLine("[Cache] First launch — SQLite cleared");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Cache] Clear failed: {ex.Message}");
            }
            Preferences.Set("db_initialized_v1", true);
        }

        return new Window(new AppShell());
    }
}
