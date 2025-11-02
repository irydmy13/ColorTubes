using Microsoft.Maui.Storage;

namespace ColorTubes.Services;

public sealed class SettingsService
{
    const string KeyLang = "Settings.Language";
    const string KeyTheme = "Settings.Theme";
    const string KeySound = "Settings.Sound";

    public string Language => Preferences.Get(KeyLang, "ru");
    public AppThemeOption Theme => (AppThemeOption)Preferences.Get(KeyTheme, (int)AppThemeOption.System);
    public bool SoundOn => Preferences.Get(KeySound, true);

    public void SaveLanguage(string lang) => Preferences.Set(KeyLang, string.IsNullOrWhiteSpace(lang) ? "ru" : lang);
    public void SaveTheme(AppThemeOption theme) => Preferences.Set(KeyTheme, (int)theme);
    public void SaveSound(bool on) => Preferences.Set(KeySound, on);
}