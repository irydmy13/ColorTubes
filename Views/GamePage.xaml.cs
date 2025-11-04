using System;
using Microsoft.Maui.Controls;
using ColorTubes.ViewModels;
using ColorTubes.Models;

namespace ColorTubes.Views;

public partial class GamePage : ContentPage
{
    private readonly GameViewModel _vm;

    public GamePage(GameViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_vm.Tubes.Count == 0)
            await _vm.StartCampaignAsync(); // стартуем уровни (5→9 колб)
    }

    private void OnTubeTapped(object sender, TappedEventArgs e)
    {
        if (sender is not VisualElement ve) return;
        if (ve.BindingContext is not Tube tube) return;
        if (_vm.SelectTubeCommand is Command<Tube> cmd && cmd.CanExecute(tube))
            cmd.Execute(tube);
    }
}
