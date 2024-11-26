using System.ComponentModel.DataAnnotations;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Data.Models;

public class Obscurity
{
    [Key]
    public int PrimaryKey { get; set; }

    public string? Description { get; set; }

    public required RoomPool RoomPool { get; set; }

    // public async Task<Room> GenerateRoom(
    //     IRoomRepository roomRepository
    // )
    // {
    //     // Pick random room from room pool
    //     var randomIndex = new Random().Next(0, RoomPool.Rooms.Count);

    //     var primaryKey = RoomPool.Rooms.ToList()[randomIndex].Room.PrimaryKey;

    //     var room = await roomRepository.FindRoom(primaryKey);

    //     var clonedRoom = room.Clone();

    //     var roomInDb = await roomRepository.CreateRoom(clonedRoom);
    //     roomInDb.Name = $"R-{roomInDb.PrimaryKey}";
    //     await roomRepository.UpdateRoom(roomInDb);

    //     return roomInDb;
    // }

    public Obscurity Clone()
    {
        return new Obscurity()
        {
            Description = this.Description,
            RoomPool = this.RoomPool
        };
    }
}
