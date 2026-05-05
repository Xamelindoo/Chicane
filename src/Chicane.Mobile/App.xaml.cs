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
        SecretsConfig.InitializeAsync().ConfigureAwait(false);
        return new Window(new AppShell());
    }
}
