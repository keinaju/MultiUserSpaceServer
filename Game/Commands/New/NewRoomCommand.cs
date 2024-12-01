using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.New;

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
        var pickedBeing = await _state.GetBeing();

        //Create and set up new room
        var room = new Room()
        {
            Name = string.Empty,
            GlobalAccess = false,
            Inventory = new Inventory()
        };

        //Connect new room to room it was created from
        room.ConnectedToRooms.Add(pickedBeing.InRoom);
        var roomInDb = await _roomRepository.CreateRoom(room);
        roomInDb.Name = $"r{roomInDb.PrimaryKey}";
        await _roomRepository.UpdateRoom(roomInDb);

        //Connect current room to new room
        var currentRoom = pickedBeing.InRoom;
        currentRoom.ConnectedToRooms.Add(roomInDb);
        await _roomRepository.UpdateRoom(currentRoom);

        //Move being to new room
        pickedBeing.InRoom = roomInDb;
        await _beingRepository.UpdateBeing(pickedBeing);

        return $"{MessageStandard.Created("room", roomInDb.Name)} {pickedBeing.Name} moved there.";
    }
}
