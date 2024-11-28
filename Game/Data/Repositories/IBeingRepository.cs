using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public interface IBeingRepository
{
    Task<Being> CreateBeing(Being being);
    Task DeleteBeing(int primaryKey);
    Task<Being> FindBeing(int primaryKey);
    Task<Being?> FindBeing(string beingName);
    Task<List<Being>> FindBeingsByUser(User user);
    Task UpdateBeing(Being being);
}
