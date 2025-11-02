using ColorTubes.ViewModels;

namespace ColorTubes.Views;

public partial class LevelsPage : ContentPage
{
    public LevelsPage(LevelsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm; 
    }
}