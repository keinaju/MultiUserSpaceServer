using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public interface ITickCounterRepository
{
    Task<TickCounter?> GetTickCount();
    Task SetTickCount(TickCounter tickCounter);
}
