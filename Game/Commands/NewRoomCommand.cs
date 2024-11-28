using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Commands;

public class NewRoomCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasPickedBeing,
    ];

    private readonly IBeingRepository _beingRepository;
    private readonly IPlayerState _state;
    private readonly IRoomRepository _roomRepository;

    public NewRoomCommand(
        IBeingRepository beingRepository,
        IPlayerState state,
        IRoomRepository roomRepository
    )
    : base(regex: @"^new room$")
    {
        _beingRepository = beingRepository;
        _state = state;
        _roomRepository = roomRepository;
    }

    public override async Task<string> Invoke()
    {
        var pickedBeing = await _state.Being();

        //Create and set up new room
        Room room = new()
        {
            Name = string.Empty,
            Inventory = new Inventory()
        };

        //Connect new room to room it was created from
        room.ConnectedToRooms.Add(pickedBeing.Room);
        var roomInDb = await _roomRepository.CreateRoom(room);
        roomInDb.Name = $"R-{roomInDb.PrimaryKey}";
        await _roomRepository.UpdateRoom(roomInDb);

        //Connect current room to new room
        var currentRoom = pickedBeing.Room;
        currentRoom.ConnectedToRooms.Add(roomInDb);
        await _roomRepository.UpdateRoom(currentRoom);

        //Move being to new room
        pickedBeing.Room = roomInDb;
        await _beingRepository.UpdateBeing(pickedBeing);

        return $"Room '{roomInDb.Name}' was created. {pickedBeing.Name} moved there.";
    }
}
