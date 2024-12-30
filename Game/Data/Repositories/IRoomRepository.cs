using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public interface IRoomRepository
{
    Task<Room> CreateRoom(Room room);

    Task DeleteCuriosities(RoomPool roomPool);

    Task DeleteRoom(int roomId);

    Task<ICollection<Room>> FindGlobalRooms();

    Task<Room> FindRoom(int primaryKey);

    Task<Room?> FindRoom(string roomName);

    Task<Room?> GetFirstRoom();

    Task<string> GetUniqueRoomName(string roomName);

    Task<bool> RoomNameIsReserved(string roomName);

    Task UpdateRoom(Room room);
}
