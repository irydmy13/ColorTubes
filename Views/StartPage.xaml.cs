using Microsoft.Maui.Storage;

namespace ColorTubes.Views;

public partial class StartPage : ContentPage
{
    public StartPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var saved = Preferences.Get("LastPlayerName", string.Empty);
        LblName.Text = string.IsNullOrWhiteSpace(saved) ? "" : $"Игрок: {saved}";
    }

    private async void Play_Clicked(object sender, EventArgs e)
    {
        var current = Preferences.Get("LastPlayerName", string.Empty);
        var name = await DisplayPromptAsync("Игрок", "Введите ваше имя:", "OK", "Отмена",
                                            initialValue: current, maxLength: 20);
        if (string.IsNullOrWhiteSpace(name)) return;

        Preferences.Set("LastPlayerName", name.Trim());
        await Shell.Current.GoToAsync("game?tubeCount=3&levelIndex=1");
    }

    // ВАЖНО: именно async void и именно такое имя
    private async void Settings_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("settings");
    }
}