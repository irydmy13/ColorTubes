using System;
using Microsoft.Maui.Controls;

namespace ColorTubes.Views;

public partial class StartPage : ContentPage
{
    public StartPage()
    {
        InitializeComponent();
    }

    private async void Play_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///main/levels");
    }

    private async void Settings_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///main/settings");
    }
}
