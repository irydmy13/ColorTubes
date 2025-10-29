using ColorTubes.Services;

namespace ColorTubes.ViewModels;

//Набор тем для Picker'а на SettingsPage.
public static class ThemeOptions
{
    public static AppThemeOption[] All { get; } =
        new[] { AppThemeOption.System, AppThemeOption.Light, AppThemeOption.Dark };
}
