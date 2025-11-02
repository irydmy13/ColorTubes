using SQLite;
using ColorTubes.Models;

namespace ColorTubes.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _db;

    public DatabaseService()
    {
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ColorTubes.db3");
        _db = new SQLiteAsyncConnection(dbPath);
    }

    public async Task InitAsync()
    {
        await _db.CreateTableAsync<PlayerScore>();
        await _db.CreateTableAsync<Level>();

        var count = await _db.Table<Level>().CountAsync();
        if (count == 0)
        {
            for (int i = 1; i <= 5; i++)
            {
                await _db.InsertAsync(new Level
                {
                    Name = $"������� {i}",
                    LayoutJson = "[]" // �����; �������� ������ ����� ���������
                });
            }
        }
    }

    public Task<int> SaveScoreAsync(PlayerScore score) => _db.InsertAsync(score);

    public Task<List<PlayerScore>> GetTopScoresAsync(int levelIndex, int take = 20) =>
        _db.Table<PlayerScore>()
           .Where(s => s.LevelIndex == levelIndex)
           .OrderBy(s => s.TimeMs)
           .ThenBy(s => s.Moves)
           .Take(take)
           .ToListAsync();

    public Task<List<Level>> GetLevelsAsync() =>
        _db.Table<Level>().OrderBy(l => l.Id).ToListAsync();

    public Task<Level?> GetLevelAsync(int id) =>
        _db.Table<Level>().Where(l => l.Id == id).FirstOrDefaultAsync();

    public async Task<int> AddLevelAsync(Level level)
    {
        if (string.IsNullOrWhiteSpace(level.Name)) level.Name = "����� �������";
        return await _db.InsertAsync(level);
    }

    public Task<int> UpdateLevelAsync(Level level) => _db.UpdateAsync(level);

    public async Task<int> DeleteLevelAsync(int id)
    {
        var lvl = await GetLevelAsync(id);
        return lvl is null ? 0 : await _db.DeleteAsync(lvl);
    }
}