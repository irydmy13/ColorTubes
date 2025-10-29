namespace ColorTubes.Services;

public class SettingsService
{
    const string KEY_LANG = "lang";
    const string KEY_THEME = "theme";
    const string KEY_SOUND = "sound";

    public string Language { get; private set; } = "ru";
    public AppThemeOption Theme { get; private set; } = AppThemeOption.System;
    public bool SoundOn { get; private set; } = true;

    public void Load()
    {
        Language = Preferences.Get(KEY_LANG, "ru");
        Theme = Enum.TryParse(Preferences.Get(KEY_THEME, "System"), out AppThemeOption t) ? t : AppThemeOption.System;
        SoundOn = Preferences.Get(KEY_SOUND, true);
    }

    public void SaveLanguage(string lang)
    {
        Language = lang;
        Preferences.Set(KEY_LANG, lang);
    }

    public void SaveTheme(AppThemeOption theme)
    {
        Theme = theme;
        Preferences.Set(KEY_THEME, theme.ToString());
    }

    public void SaveSound(bool on)
    {
        SoundOn = on;
        Preferences.Set(KEY_SOUND, on);
    }
}
