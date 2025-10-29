using SQLite;

namespace ColorTubes.Models;

[Table("Level")]
public class Level
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(80)]
    public string Name { get; set; } = "Level";

    // JSON с раскладкой пробирок
    public string LayoutJson { get; set; } = string.Empty;
}
