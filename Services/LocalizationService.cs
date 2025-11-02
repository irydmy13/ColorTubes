using System.Globalization;
using System.Resources;

namespace ColorTubes.Services;

public sealed class LocalizationService
{
    public CultureInfo CurrentCulture { get; private set; } = CultureInfo.CurrentUICulture;

   private readonly ResourceManager _rm = Resources.Localization.AppResources.ResourceManager;

    public void SetCulture(string langCode)
    {
        if (string.IsNullOrWhiteSpace(langCode)) langCode = "ru";
        var ci = new CultureInfo(langCode);
        CultureInfo.CurrentUICulture = ci;
        CultureInfo.CurrentCulture = ci;
        CurrentCulture = ci;

        Resources.Localization.AppResources.Culture = ci;
    }

    public string T(string key) => _rm.GetString(key, CurrentCulture) ?? key;
}