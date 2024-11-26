using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Commands;

public class AddRoomInRoomPoolCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn
    ];

    private readonly IRoomRepository _roomRepository;
    private readonly IRoomPoolRepository _roomPoolRepository;

    private string RoomName => GetParameter(1);
    private string RoomPoolName => GetParameter(2);

    public AddRoomInRoomPoolCommand(
        IRoomRepository roomRepository,
        IRoomPoolRepository roomPoolRepository
    )
    : base(regex: @"^add (.*) in room pool (.*)$")
    {
        _roomRepository = roomRepository;
        _roomPoolRepository = roomPoolRepository;
    }

    public override async Task<string> Invoke()
    {
        var room = await _roomRepository.FindRoom(RoomName);
        if(room is null)
        {
            return $"{RoomName} does not exist.";
        }

        var roomPool = 
            await _roomPoolRepository.FindRoomPool(RoomPoolName);
        if(roomPool is null)
        {
            return $"{RoomPoolName} does not exist.";
        }

        // Prevent adding duplicate rooms in room pool
        foreach(var roomInPool in roomPool.RoomsInPool)
        {
            if(roomInPool.Room.PrimaryKey == room.PrimaryKey)
            {
                return $"{RoomName} is already in room pool {roomPool.Name}.";
            }
        }

        roomPool.RoomsInPool.Add(new RoomInPool() {
            Room = room
        });
        await _roomPoolRepository.UpdateRoomPool(roomPool);

        return $"{room.Name} was added to room pool {roomPool.Name}.";
    }
}
