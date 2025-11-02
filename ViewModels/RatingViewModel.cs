using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ColorTubes.Models;
using ColorTubes.Services;

namespace ColorTubes.ViewModels;

public class RatingItem
{
    public int Rank { get; set; }
    public string PlayerName { get; set; } = "";
    public int Moves { get; set; }
    public string Time { get; set; } = "";
}

public class RatingViewModel : BaseViewModel
{
    private readonly DatabaseService _db = new();

    public ObservableCollection<RatingItem> Scores { get; } = new();

    public ICommand RefreshCommand { get; }

    public RatingViewModel()
    {
        RefreshCommand = new Command(async () => await LoadAsync());
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        Scores.Clear();

        var top = await _db.GetTopScoresAsync();
        int rank = 1;

        foreach (var s in top)
        {
            Scores.Add(new RatingItem
            {
                Rank = rank++,
                PlayerName = string.IsNullOrWhiteSpace(s.PlayerName) ? "Игрок" : s.PlayerName,
                Moves = s.Moves,
                Time = s.PlayTime.ToString()
            });
        }
    }
}

