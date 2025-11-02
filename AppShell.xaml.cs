using ColorTubes.Views;

namespace ColorTubes;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("game", typeof(GamePage));
        Routing.RegisterRoute("levels", typeof(LevelsPage));
        Routing.RegisterRoute("settings", typeof(SettingsPage));
    }
}