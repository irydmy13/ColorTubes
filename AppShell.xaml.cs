using ColorTubes.Views;

namespace ColorTubes;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("game", typeof(Views.GamePage));
    }
}