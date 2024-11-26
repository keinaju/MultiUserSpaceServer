using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public interface IGameSettingsRepository
{
    Task<GameSettings?> GetGameSettings();
    Task SetGameSettings(GameSettings settings);
}
