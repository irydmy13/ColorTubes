using SQLite;

namespace ColorTubes.Models;

[Table("PlayerScore")]
public class PlayerScore
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int LevelIndex { get; set; }   // 1..5

    [MaxLength(40)]
    public string PlayerName { get; set; } = "Player";
    public double PlayTime { get; set; }
    public int Moves { get; set; }
    public long TimeMs { get; set; }  
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}