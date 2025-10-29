using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ColorTubes.ViewModels;

public class LevelItem
{
    public int Index { get; set; }          // 1..5
    public int TubeCount { get; set; }      // 3..7
    public string Title => $"”ровень {Index} Ч {TubeCount} пробирок";
}

public class LevelsViewModel : BaseViewModel
{
    public ObservableCollection<LevelItem> Levels { get; } = new();
    public ICommand OpenLevelCommand { get; }

    public LevelsViewModel()
    {
        for (int i = 0; i < 5; i++)
            Levels.Add(new LevelItem { Index = i + 1, TubeCount = 3 + i });

        OpenLevelCommand = new Command<LevelItem>(async lvl =>
        {
            if (lvl == null) return;
            await Shell.Current.GoToAsync($"game?tubeCount={lvl.TubeCount}&levelIndex={lvl.Index}");
        });
    }
}
