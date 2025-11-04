using System.Globalization;
using System.Threading;
using Microsoft.Maui.Storage;
using ColorTubes.Resources.Localization; // AppResources

namespace ColorTubes;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // читаем сохранённый язык (по умолчанию ru)
        var code = Preferences.Get("lang", "ru");
        ApplyCulture(code);

        MainPage = new AppShell();
    }

    public static void ApplyCulture(string code)
    {
        try
        {
            var culture = new CultureInfo(code);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            AppResources.Culture = culture; // ключевая строка
        }
        catch
        {
            // если код кривой — вернёмся к ru
            var culture = new CultureInfo("ru");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            AppResources.Culture = culture;
        }
    }

    public static void RestartShell()
    {
        // пересоздаём корневую оболочку, чтобы обновились x:Static ресурсы
        Current!.MainPage = new AppShell();
    }
}
