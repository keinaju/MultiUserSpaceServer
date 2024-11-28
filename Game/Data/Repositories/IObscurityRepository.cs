using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public interface ICuriosityRepository
{
    Task<Curiosity> FindCuriosity(int primaryKey);
}
