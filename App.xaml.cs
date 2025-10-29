using ColorTubes.Services;

namespace ColorTubes;

public partial class App : Application
{
    public App(ThemeService theme, LocalizationService loc, SettingsService settings, AudioService audio)
    {
        InitializeComponent();

        settings.Load();
        theme.SetTheme(settings.Theme);
        loc.SetCulture(settings.Language);

        // Инициализируем звуки один раз
        _ = audio.InitAsync();

        MainPage = new AppShell();
    }
}
