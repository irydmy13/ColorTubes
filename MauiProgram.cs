using ColorTubes.Services;

namespace ColorTubes;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(f =>
            {
                f.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                f.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Сервисы
        builder.Services.AddSingleton<ThemeService>();
        builder.Services.AddSingleton<SettingsService>();
        builder.Services.AddSingleton<LocalizationService>();
        builder.Services.AddSingleton<DatabaseService>();

        // ViewModels
        builder.Services.AddSingleton<ViewModels.SettingsViewModel>();
        builder.Services.AddSingleton<ViewModels.LevelsViewModel>();
        builder.Services.AddTransient<ViewModels.GameViewModel>();

        // Pages
        builder.Services.AddSingleton<Views.SettingsPage>();
        builder.Services.AddSingleton<Views.LevelsPage>();
        builder.Services.AddTransient<Views.GamePage>();

        var app = builder.Build();

        var db = app.Services.GetRequiredService<DatabaseService>();
        Task.Run(db.InitAsync).Wait();

        return app;
    }
}
