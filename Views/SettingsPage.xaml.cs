using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace ColorTubes.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // выставим текущий €зык в пикере
        var code = Preferences.Get("lang", "ru");
        LangPicker.SelectedIndex = code switch
        {
            "ru" => 0,
            "et" => 1,
            "en" => 2,
            _ => 0
        };
    }

    private async void OnApplyLangClicked(object sender, EventArgs e)
    {
        if (LangPicker.SelectedItem is not string item) return;

        // строка вида "ru Ч –усский"
        var code = item.Split('Ч')[0].Trim().ToLowerInvariant();

        Preferences.Set("lang", code);
        App.ApplyCulture(code);
        App.RestartShell(); // обновл€ем все x:Static ресурсы на страницах

        // маленький визуальный отклик
        if (HintLabel is not null)
        {
            HintLabel.Text = "язык применЄн";
            await HintLabel.FadeTo(1, 150);
            await Task.Delay(700);
            await HintLabel.FadeTo(0.0, 200);
        }
    }
}
