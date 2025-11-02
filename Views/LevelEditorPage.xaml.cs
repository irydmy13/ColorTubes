using ColorTubes.Models;
using ColorTubes.Services;
using ColorTubes.Utils;

namespace ColorTubes.Views;

[QueryProperty(nameof(LevelId), "LevelId")]
public partial class LevelEditorPage : ContentPage
{
    private readonly DatabaseService _db;
    private Level? _level;

    public LevelEditorPage(DatabaseService db)
    {
        InitializeComponent();
        _db = db;
    }

    public int LevelId { get; set; }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var list = await _db.GetLevelsAsync();
        _level = list.FirstOrDefault(l => l.Id == LevelId);
        if (_level is null) return;

        Title = $"Редактор: {_level.Name}";
        NameEntry.Text = _level.Name;
        JsonEditor.Text = _level.LayoutJson;
        StatusLabel.Text = "Измени имя и/или JSON, затем нажми «Сохранить».";
    }

    private void OnValidateClicked(object sender, EventArgs e)
    {
        try
        {
            _ = LevelLayouts.FromJson(JsonEditor.Text);
            StatusLabel.Text = "JSON валиден ✅";
            StatusLabel.TextColor = Colors.ForestGreen;
        }
        catch (Exception ex)
        {
            StatusLabel.Text = "Ошибка JSON: " + ex.Message;
            StatusLabel.TextColor = Colors.IndianRed;
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (_level is null) return;
        try
        {
            // проверяем парсинг
            var parsed = LevelLayouts.FromJson(JsonEditor.Text);
            // если ок — сохраняем
            _level.Name = NameEntry.Text?.Trim() ?? _level.Name;
            _level.LayoutJson = LevelLayouts.ToJson(parsed);
            await _db.UpdateLevelAsync(_level);

            await DisplayAlert("Сохранено", "Уровень обновлён.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось сохранить: {ex.Message}", "OK");
        }
    }
}