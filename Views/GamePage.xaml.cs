using ColorTubes.ViewModels;
using ColorTubes.Models;

namespace ColorTubes.Views;
public partial class GamePage : ContentPage
{
    public GamePage()
    {
        InitializeComponent();
    }

private void OnTubeTapped(object sender, TappedEventArgs e)
    {
        if (BindingContext is not GameViewModel vm) return;
        if (sender is not VisualElement ve) return;
        if (ve.BindingContext is not Tube tube) return;

        if (vm.SelectTubeCommand is Command<Tube> cmd && cmd.CanExecute(tube))
            cmd.Execute(tube);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is GameViewModel vm)
        {
            if (vm.Tubes == null || vm.Tubes.Count == 0)
                await vm.StartCampaignAsync(); // старт кампании 5 уровней
        }
    }
}