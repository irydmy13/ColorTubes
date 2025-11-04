using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace ColorTubes.Views;

public partial class StartPage : ContentPage
{
    public StartPage()
    {
        InitializeComponent();
    }

    private async void Play_Clicked(object sender, EventArgs e)
    {
        var current = Preferences.Get("PlayerName", "");
        var name = await DisplayPromptAsync("Игрок", "Введи своё имя:", initialValue: current, maxLength: 20);
        if (string.IsNullOrWhiteSpace(name)) return;

        Preferences.Set("PlayerName", name.Trim());
        await Shell.Current.GoToAsync("///main/menu");
        await Shell.Current.GoToAsync("game");
    }
    private async void OnRatingClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///main/rating");
    }
    private async void Settings_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///main/settings");
    }
}
