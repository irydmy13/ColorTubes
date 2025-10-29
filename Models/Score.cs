using SQLite;

namespace ColorTubes.Models;

public class Score
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public int LevelId { get; set; }
    public int Moves { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
}
