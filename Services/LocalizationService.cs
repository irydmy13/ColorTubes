using System.Globalization;
using ColorTubes.Resources.Localization;

namespace ColorTubes.Services;
public class LocalizationService
{
    public CultureInfo CurrentCulture { get; private set; } = CultureInfo.GetCultureInfo("ru");

    public void SetCulture(string lang)
    {
        CurrentCulture = CultureInfo.GetCultureInfo(lang);
        AppResources.Culture = CurrentCulture;

        Thread.CurrentThread.CurrentUICulture = CurrentCulture;
        Thread.CurrentThread.CurrentCulture = CurrentCulture;

        // Если нужно оповещать страницы о смене — можно добавить MessagingCenter/WeakReferenceMessenger
    }

    public string T(string key) =>
        AppResources.ResourceManager.GetString(key, AppResources.Culture) ?? key;
}
