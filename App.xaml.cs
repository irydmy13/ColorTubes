namespace ColorTubes;

public partial class App : Application
{
    private readonly Services.ThemeService _themeService;

    // App получает ThemeService через DI
    public App(Services.ThemeService themeService)
    {
        InitializeComponent();

        _themeService = themeService;
        _themeService.ApplySavedTheme(); // применяем тему при запуске

        MainPage = new AppShell();
    }
}