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
        if (BindingContext is GameViewModel vm)
            await vm.EnsureStartedAsync();
    }
}