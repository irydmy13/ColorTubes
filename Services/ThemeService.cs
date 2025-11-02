using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace ColorTubes.Services;

public enum AppThemeOption { System = 0, Light = 1, Dark = 2 }

public sealed class ThemeService
{
    private const string Key = "AppThemeOption";

    public AppThemeOption CurrentOption
    {
        get => (AppThemeOption)Preferences.Get(Key, (int)AppThemeOption.System);
        private set => Preferences.Set(Key, (int)value);
    }

 public void ApplySavedTheme()
    {
        ApplyTheme(CurrentOption);
    }

   public void SetTheme(AppThemeOption option)
    {
        CurrentOption = option;
        ApplyTheme(option);
    }

    private static void ApplyTheme(AppThemeOption option)
    {
        var app = Application.Current;
        if (app is null) return;

        app.UserAppTheme = option switch
        {
            AppThemeOption.Light => AppTheme.Light,
            AppThemeOption.Dark => AppTheme.Dark,
            _ => AppTheme.Unspecified, // System
        };
    }
}