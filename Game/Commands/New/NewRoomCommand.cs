using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.New;

public class NewRoomCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder,
        Prerequisite.UserHasSelectedBeing,
    ];

    protected override string Description =>
    "Creates a new room and connects it to the current room.";

    private readonly IBeingRepository _beingRepository;
    private readonly IGameResponse _response;
    private readonly IPlayerState _state;
    private readonly IRoomRepository _roomRepository;

    public NewRoomCommand(
        IBeingRepository beingRepository,
        IGameResponse response,
        IPlayerState state,
        IRoomRepository roomRepository
    )
    : base(regex: @"^new room$")
    {
        _beingRepository = beingRepository;
        _response = response;
        _state = state;
        _roomRepository = roomRepository;
    }

    public override async Task Invoke()
    {
        var selectedBeing = await _state.GetBeing();

        var oldRoom = await _roomRepository.FindRoom(
            selectedBeing.InRoom.PrimaryKey
        );

        // Initialize an empty room
        var newRoom = new Room()
        {
            GlobalAccess = false,
            Name = string.Empty,
            Inventory = new Inventory()
        };

        //Connect new room to room it was created from
        newRoom.ConnectedToRooms.Add(oldRoom);
        newRoom = await _roomRepository.CreateRoom(newRoom);
        newRoom.Name = $"r{newRoom.PrimaryKey}";
        await _roomRepository.UpdateRoom(newRoom);

        //Connect old room to new room
        oldRoom.ConnectedToRooms.Add(newRoom);
        await _roomRepository.UpdateRoom(oldRoom);

        //Move being to new room
        selectedBeing.InRoom = newRoom;
        await _beingRepository.UpdateBeing(selectedBeing);

        _response.AddText(
            $"{MessageStandard.Created("room", newRoom.Name)}"
        );
        _response.AddText(
            $"{selectedBeing.Name} moved to {newRoom.Name}."
        );
    }
}
