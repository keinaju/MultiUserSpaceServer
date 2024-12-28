using Microsoft.EntityFrameworkCore;
using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public class GameSettingsRepository : IGameSettingsRepository
{
    private readonly GameContext _context;

    public GameSettingsRepository(GameContext context)
    {
        _context = context;
    }

    public async Task<GameSettings?> GetGameSettings()
    {
        try
        {
            return await _context.GameSettings
            .FirstAsync();
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    public async Task SetGameSettings(GameSettings settings)
    {
        var settingsInDb = await GetGameSettings();

        if (settingsInDb is null)
        {
            await _context.GameSettings.AddAsync(settings);
        }
        else
        {
            settingsInDb = settings;
        }
        
        await _context.SaveChangesAsync();
    }
}
