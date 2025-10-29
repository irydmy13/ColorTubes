using System.Globalization;
using System.Resources;

namespace ColorTubes.Services;

// Простая локализация через .resx. Меняет культуру на лету
public sealed class LocalizationService
{
    public CultureInfo CurrentCulture { get; private set; } = CultureInfo.CurrentUICulture;

    // если используешь AppResources.resx
    private readonly ResourceManager _rm = Resources.Localization.AppResources.ResourceManager;

    public void SetCulture(string langCode)
    {
        if (string.IsNullOrWhiteSpace(langCode)) langCode = "ru";
        var ci = new CultureInfo(langCode);
        CultureInfo.CurrentUICulture = ci;
        CultureInfo.CurrentCulture = ci;
        CurrentCulture = ci;

        // для статических ресурсов, если нужно:
        Resources.Localization.AppResources.Culture = ci;
    }

    public string T(string key) => _rm.GetString(key, CurrentCulture) ?? key;
}
