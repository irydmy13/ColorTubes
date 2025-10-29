using ColorTubes.Helpers;
using ColorTubes.ViewModels;

namespace ColorTubes.Views;

public partial class GamePage : ContentPage
{
    public GamePage()
    {
        InitializeComponent();
        BindingContext = ServiceHelper.GetService<GameViewModel>();
    }
}
