using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands;

public class GoCommand : BaseCommand
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

    public GoCommand(
        IBeingRepository beingRepository,
        IRoomRepository roomRepository,
        IPlayerState state
    )
    : base(regex: @"^go (global |connected |being )?(.+)$")
    {
        _beingRepository = beingRepository;
        _roomRepository = roomRepository;
        _state = state;
    }

    public override async Task<string> Invoke()
    {
        Room current = await _state.GetRoom();
        Room? destination = null;

        // Go to command has 4 strategies to find rooms
        switch(PlaceParameter)
        {
            case "":
                // 1. No parameter is to look for everywhere.
                // In case of duplicates, first match is returned.
                // Find first from connected rooms.
                // If not found, find room from inside beings.
                // If not found, find room with global access.
                destination = await FindConnectedRoom();
                if(destination is null) destination = await FindInsideRoom();
                if(destination is null) destination = await FindGlobalRoom();
                break;

            case "global":
                // 2. Global parameter is to look for rooms with global access only. 
                destination = await FindGlobalRoom();
                break;

            case "connected":
                // 3. Connected parameter => Look only for rooms connected to current room.
                destination = await FindConnectedRoom();
                break;

            case "being":
                // 4. Being parameter => Look only for rooms inside beings.
                destination = await FindInsideRoom();
                break;
        }

        if(destination is null)
        {
            return MessageStandard.DoesNotExist("Room", RoomNameInUserInput);
        }

        // Populate
        destination = await _roomRepository.FindRoom(destination.PrimaryKey);

        var being = await _state.GetBeing();

        if(destination.PrimaryKey == current.PrimaryKey)
        {
            return $"{being.Name} is in {destination.Name}.";
        }

        foreach(var requiredFeature in destination.BeingMustHaveFeatures)
        {
            if(!being.Features.Contains(requiredFeature))
            {
                return $"{being.Name} can not enter {destination.Name} " +
                $"without {requiredFeature.Name} feature.";
            }
        }

        await _state.Move(destination);
        return $"{being.Name} moved to {destination.Name}.";
    }

    private async Task<Room?> FindGlobalRoom()
    {
        var room = await _roomRepository.FindRoom(RoomNameInUserInput);
        
        if (room is null)
        {
            return null;
        }

        if (room.GlobalAccess == true)
        {
            return room;
        }

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
        var thisBeing = await _state.GetBeing();

        foreach(var beingHere in thisRoom.BeingsHere)
        {
            // Don't allow being to access it's own inside room
            if (beingHere.PrimaryKey == thisBeing.PrimaryKey)
            {
                continue;
            }

            var populatedBeing = 
                await _beingRepository.FindBeing(beingHere.PrimaryKey);

            var insideRoom = populatedBeing.RoomInside;

            if (insideRoom is null)
            {
                continue;
            }

            if (populatedBeing.Name == RoomNameInUserInput)
            {
                return insideRoom;
            }
        }

        return null;
    }
}
