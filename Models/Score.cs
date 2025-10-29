using SQLite;

namespace ColorTubes.Models;

[Table("scores")]
public class Score
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    [Indexed] public int LevelNumber { get; set; }
    [Indexed] public string PlayerName { get; set; } = "";
    public double TimeSeconds { get; set; }
    public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
}
