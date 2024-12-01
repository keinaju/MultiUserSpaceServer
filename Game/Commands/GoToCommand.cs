using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands;

public class GoToCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasPickedBeing
    ];

    private readonly IBeingRepository _beingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IPlayerState _state;

    private string PlaceParameter => GetParameter(1).Trim();
    private string RoomNameInUserInput => GetParameter(2);

    public GoToCommand(
        IBeingRepository beingRepository,
        IRoomRepository roomRepository,
        IPlayerState state
    )
    : base(regex: @"^go to (global |connected |being )?(.+)$")
    {
        _beingRepository = beingRepository;
        _roomRepository = roomRepository;
        _state = state;
    }

    public override async Task<string> Invoke()
    {
        Room currentRoom = await _state.GetRoom();

        Room? destinationRoom = null;

        if(PlaceParameter == "")
        {
            destinationRoom = await FindConnectedRoom();
            if(destinationRoom is null) destinationRoom = await FindInsideRoom();
            if(destinationRoom is null) destinationRoom = await FindGlobalRoom();
        }
        else if(PlaceParameter == "global") destinationRoom = await FindGlobalRoom();
        else if(PlaceParameter == "connected") destinationRoom = await FindConnectedRoom();
        else if(PlaceParameter == "being") destinationRoom = await FindInsideRoom();

        if(destinationRoom is null)
        {
            return MessageStandard.DoesNotExist(RoomNameInUserInput);
        }

        var being = await _state.GetBeing();

        if(destinationRoom.PrimaryKey == currentRoom.PrimaryKey)
        {
            return $"{being.Name} is in {destinationRoom.Name}.";
        }

        await _state.Move(destinationRoom);
        return $"{being.Name} moved to {destinationRoom.Name}.";
    }

    private async Task<Room?> FindGlobalRoom()
    {
        var room = await _roomRepository.FindRoom(RoomNameInUserInput);
        
        if (room is null) return null;

        if(room.GlobalAccess == true) return room;

        return null;
    }

    private async Task<Room?> FindConnectedRoom()
    {
        var thisRoom = await _state.GetRoom();
        foreach(var connectedRoom in thisRoom.ConnectedToRooms)
        {
            if (connectedRoom.Name == RoomNameInUserInput) return connectedRoom;
        }

        return null;
    }

    private async Task<Room?> FindInsideRoom()
    {
        var thisRoom = await _state.GetRoom();
        foreach(var beingHere in thisRoom.BeingsHere)
        {
            var populatedBeing = await _beingRepository
                .FindBeing(beingHere.PrimaryKey);

            var insideRoom = populatedBeing.RoomInside;

            if (insideRoom is null) continue;

            if (populatedBeing.Name == RoomNameInUserInput) return insideRoom;
        }
        return null;
    }
}
