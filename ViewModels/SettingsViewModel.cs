using System.Windows.Input;
using ColorTubes.Services;

namespace ColorTubes.ViewModels;

// VM для страницы настроек. Хранит выбранный язык/тему/звук и сохраняет их.

public class SettingsViewModel : BaseViewModel
{
    private readonly SettingsService _settings;
    private readonly ThemeService _theme;
    private readonly LocalizationService _loc;

    private string _language;
    private AppThemeOption _themeOption;
    private bool _soundOn;

    public string Language
    {
        get => _language;
        set => Set(ref _language, value);
    }

    public AppThemeOption Theme
    {
        get => _themeOption;
        set => Set(ref _themeOption, value);
    }

    public bool SoundOn
    {
        get => _soundOn;
        set => Set(ref _soundOn, value);
    }

    public ICommand SaveCommand { get; }

    public SettingsViewModel(SettingsService s, ThemeService t, LocalizationService l)
    {
        _settings = s; _theme = t; _loc = l;

        _language = s.Language;
        _themeOption = s.Theme;
        _soundOn = s.SoundOn;

        SaveCommand = new Command(SaveAndApply);
    }

    private void SaveAndApply()
    {
        _settings.SaveLanguage(Language);
        _settings.SaveTheme(Theme);
        _settings.SaveSound(SoundOn);

        _theme.SetTheme(Theme);
        _loc.SetCulture(Language);
    }
}
