using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public interface IRoomRepository
{
    Task<Room> CreateRoom(Room room);
    Task DeleteRoom(int roomId);
    Task<Room> FindRoom(int primaryKey);
    Task<Room?> FindRoom(string roomName);
    Task<Room?> GetFirstRoom();
    Task UpdateRoom(Room room);
}
