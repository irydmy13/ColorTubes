using SQLite;
using ColorTubes.Models;

namespace ColorTubes.Services;

//Простая обёртка над SQLite. Хранит уровни и рекорды.
public class DatabaseService
{
    private readonly SQLiteAsyncConnection _db;

    public DatabaseService()
    {
        var path = Path.Combine(FileSystem.AppDataDirectory, "colortubes.db");
        _db = new SQLiteAsyncConnection(path);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await _db.CreateTableAsync<Level>();
        await _db.CreateTableAsync<Score>();
    }

    // ---- Levels (CRUD)
    public Task<List<Level>> GetLevelsAsync() => _db.Table<Level>().OrderBy(l => l.Id).ToListAsync();
    public Task<int> AddLevelAsync(Level l) => _db.InsertAsync(l);
    public Task<int> UpdateLevelAsync(Level l) => _db.UpdateAsync(l);
    public Task<int> DeleteLevelAsync(Level l) => _db.DeleteAsync(l);

    // ---- Scores (пригодится позже)
    public Task<List<Score>> GetScoresAsync() => _db.Table<Score>().OrderByDescending(s => s.Moves).ToListAsync();
    public Task<int> AddScoreAsync(Score s) => _db.InsertAsync(s);

    // ---- Утиль
    public async Task EnsureSampleLevelAsync()
    {
        if ((await _db.Table<Level>().CountAsync()) == 0)
        {
            var sample = LevelLayouts.SampleLevel();
            await _db.InsertAsync(sample);
        }
    }
}
