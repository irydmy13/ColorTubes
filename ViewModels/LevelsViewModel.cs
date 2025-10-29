using System.Collections.ObjectModel;
using System.Windows.Input;
using ColorTubes.Models;
using ColorTubes.Services;
using ColorTubes.Views;

namespace ColorTubes.ViewModels;

public class LevelsViewModel : BaseViewModel
{
    private readonly DatabaseService _db;

    public ObservableCollection<Level> Levels { get; } = new();

    public ICommand LoadCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand EditCommand { get; }

    public LevelsViewModel(DatabaseService db)
    {
        _db = db;
        LoadCommand = new Command(async () => await LoadAsync());
        AddCommand = new Command(async () =>
        {
            var l = LevelLayouts.SampleLevel();
            l.Name = "Новый уровень";
            await _db.AddLevelAsync(l);
            await LoadAsync();
        });
        DeleteCommand = new Command<Level>(async l =>
        {
            if (l is null) return;
            await _db.DeleteLevelAsync(l);
            await LoadAsync();
        });
        EditCommand = new Command<Level>(async l =>
        {
            if (l is null) return;
            var dict = new Dictionary<string, object> { ["LevelId"] = l.Id };
            await Shell.Current.GoToAsync(nameof(LevelEditorPage), dict);
        });
    }

    public async Task LoadAsync()
    {
        Levels.Clear();
        await _db.EnsureSampleLevelAsync();
        foreach (var l in await _db.GetLevelsAsync()) Levels.Add(l);
    }
}
