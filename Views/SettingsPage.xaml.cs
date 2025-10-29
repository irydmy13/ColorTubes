using ColorTubes.Helpers;
using ColorTubes.ViewModels;

namespace ColorTubes.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        BindingContext = ServiceHelper.GetService<SettingsViewModel>();
    }

    // Меняем язык и сразу применяем настройки (вызов SaveCommand)
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
}
