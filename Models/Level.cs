using SQLite;

namespace ColorTubes.Models;

//Уровень: имя + LayoutJson (список пробирок и слоёв)
public class Level
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public string Name { get; set; } = "Level";
    public string LayoutJson { get; set; } = "[]";
}
