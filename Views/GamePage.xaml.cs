using ColorTubes.ViewModels;

namespace ColorTubes.Views;

[QueryProperty(nameof(TubeCount), "tubeCount")]
[QueryProperty(nameof(LevelIndex), "levelIndex")]
public partial class GamePage : ContentPage
{
    public int TubeCount { get; set; } = 3;
    public int LevelIndex { get; set; } = 1;

    public GamePage(GameViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var vm = (GameViewModel)BindingContext;

        // Имя игрока
        string? name = Preferences.Get("LastPlayerName", null);
        if (string.IsNullOrWhiteSpace(name))
        {
            name = await DisplayPromptAsync("Игрок", "Введите ваше имя:", "OK", "Отмена", null, 20);
            if (string.IsNullOrWhiteSpace(name)) name = "Player";
            Preferences.Set("LastPlayerName", name);
        }

        await vm.StartNewGameAsync(TubeCount, LevelIndex, name!);
    }
}
