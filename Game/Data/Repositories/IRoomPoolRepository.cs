using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public interface IRoomPoolRepository
{
    Task<RoomPool> CreateRoomPool(RoomPool roomPool);

    Task<RoomPool> FindRoomPool(int primaryKey);

    Task<RoomPool?> FindRoomPool(string roomPoolName);

    Task<ICollection<RoomPool>> FindRoomPools();

    Task<string> GetUniqueRoomPoolName(string roomPoolName);

    Task<bool> RoomPoolNameIsReserved(string roomPoolName);

    Task UpdateRoomPool(RoomPool updatedRoomPool);
}
