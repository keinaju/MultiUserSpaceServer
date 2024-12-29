using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public interface IBeingRepository
{
    Task<bool> BeingNameIsReserved(string beingName);
    
    Task<Being> CreateBeing(Being being);

    Task DeleteBeing(int primaryKey);

    Task<Being> FindBeing(int primaryKey);

    Task<Being?> FindBeing(string beingName);

    Task<Being?> FindBeingByRoomInside(Room room);

    Task<List<Being>> FindBeingsByUser(User user);

    Task<string> GetUniqueBeingName(string beingName);

    Task UpdateBeing(Being being);
}
