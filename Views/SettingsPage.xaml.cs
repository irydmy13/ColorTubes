using ColorTubes.Helpers;
using ColorTubes.Services;
using ColorTubes.ViewModels;

namespace ColorTubes.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        // ВМ получаем через ServiceHelper — как у тебя и было
        BindingContext = ServiceHelper.GetService<SettingsViewModel>();
    }

    // ----- ЯЗЫК -----
    private void LangRu_Clicked(object? sender, EventArgs e)
    {
        var vm = (SettingsViewModel)BindingContext;
        vm.Language = "ru";
        vm.SaveCommand.Execute(null);
    }

    private void LangEn_Clicked(object? sender, EventArgs e)
    {
        var vm = (SettingsViewModel)BindingContext;
        vm.Language = "en";
        vm.SaveCommand.Execute(null);
    }

    private void LangEt_Clicked(object? sender, EventArgs e)
    {
        var vm = (SettingsViewModel)BindingContext;
        vm.Language = "et";
        vm.SaveCommand.Execute(null);
    }

    // ----- ТЕМА -----
    private void ThemeSystem_Clicked(object? sender, EventArgs e)
    {
        var vm = (SettingsViewModel)BindingContext;
        vm.Theme = AppThemeOption.System;
        vm.SaveCommand.Execute(null);
    }

    private void ThemeLight_Clicked(object? sender, EventArgs e)
    {
        var vm = (SettingsViewModel)BindingContext;
        vm.Theme = AppThemeOption.Light;
        vm.SaveCommand.Execute(null);
    }

    private void ThemeDark_Clicked(object? sender, EventArgs e)
    {
        var vm = (SettingsViewModel)BindingContext;
        vm.Theme = AppThemeOption.Dark;
        vm.SaveCommand.Execute(null);
    }
}
