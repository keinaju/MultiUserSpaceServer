using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands;

public class AddRoomInRoomPoolCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder
    ];

    private readonly IGameResponse _response;
    private readonly IRoomRepository _roomRepository;
    private readonly IRoomPoolRepository _roomPoolRepository;

    private string RoomName => GetParameter(1);
    private string RoomPoolName => GetParameter(2);

    protected override string Description =>
        "Adds a room in a room pool.";

    public AddRoomInRoomPoolCommand(
        IGameResponse response,
        IRoomRepository roomRepository,
        IRoomPoolRepository roomPoolRepository
    )
    : base(regex: @"^add (.+) in room pool (.+)$")
    {
        _response = response;
        _roomRepository = roomRepository;
        _roomPoolRepository = roomPoolRepository;
    }

    public override async Task Invoke()
    {
        var room = await _roomRepository.FindRoom(RoomName);
        if(room is null)
        {
            _response.AddText(
                MessageStandard.DoesNotExist("Room", RoomName)
            );
            return;
        }

        var roomPool = await _roomPoolRepository
        .FindRoomPool(RoomPoolName);
        if(roomPool is null)
        {
            _response.AddText(
                MessageStandard.DoesNotExist("Room pool", RoomPoolName)
            );
            return;
        }

        // Prevent adding duplicate rooms in room pool
        foreach(var roomInPool in roomPool.RoomsInPool)
        {
            if(roomInPool.Room.PrimaryKey == room.PrimaryKey)
            {
                _response.AddText(
                    $"{RoomName} is already in room pool {roomPool.Name}."
                );
                return;
            }
        }

        roomPool.RoomsInPool.Add(
            new RoomInPool() { Room = room }
        );
        await _roomPoolRepository.UpdateRoomPool(roomPool);

        _response.AddText($"{room.Name} was added to room pool {roomPool.Name}.");
    }
}
