namespace ColorTubes.Services;

public enum AppThemeOption { System, Light, Dark }

public class ThemeService
{
    public void SetTheme(AppThemeOption option)
    {
        Application.Current!.UserAppTheme = option switch
        {
            AppThemeOption.Light => AppTheme.Light,
            AppThemeOption.Dark => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
    }
}
