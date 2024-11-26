using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public interface IObscurityRepository
{
    Task<Obscurity> FindObscurity(int primaryKey);
}
