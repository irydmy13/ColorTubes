using ColorTubes.Helpers;
using ColorTubes.ViewModels;

namespace ColorTubes.Views;

public partial class LevelsPage : ContentPage
{
    public LevelsPage()
    {
        InitializeComponent();
        BindingContext = ServiceHelper.GetService<LevelsViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is LevelsViewModel vm)
            await vm.LoadAsync();
    }
}
