using ColorTubes.Services;
using ColorTubes.ViewModels;
using ColorTubes.Views;

namespace ColorTubes;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            // .UseMauiCommunityToolkit() // если используешь Toolkit
            .ConfigureFonts(fonts =>
            {
                // fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // ---- Сервисы (Singleton)
        builder.Services.AddSingleton<DatabaseService>();      // если ещё не создал — потом добавишь реализацию
        builder.Services.AddSingleton<ThemeService>();
        builder.Services.AddSingleton<LocalizationService>();
        builder.Services.AddSingleton<AudioService>();
        builder.Services.AddSingleton<SettingsService>();

        // ---- VM + Views
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<SettingsPage>();

        // Заглушки для остальных страниц (если уже созданы — DI подключит их)
        builder.Services.AddTransient<GameViewModel>();
        builder.Services.AddTransient<GamePage>();
        builder.Services.AddTransient<LevelsViewModel>();
        builder.Services.AddTransient<LevelsPage>();
        builder.Services.AddTransient<LevelEditorPage>();

        return builder.Build();
    }
}
