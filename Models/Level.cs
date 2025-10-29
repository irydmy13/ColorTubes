using SQLite;

namespace ColorTubes.Models;

//�������: ��� + LayoutJson (������ �������� � ����)
public class Level
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public string Name { get; set; } = "Level";
    public string LayoutJson { get; set; } = "[]";
}
