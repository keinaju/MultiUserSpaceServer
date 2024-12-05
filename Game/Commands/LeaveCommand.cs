using System;
using MUS.Game.Data;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Commands;

public class LeaveCommand : BaseCommand
{
    private readonly IBeingRepository _beingRepository;
    private readonly IPlayerState _state;

    public LeaveCommand(
        IBeingRepository beingRepository,
        IPlayerState state
    )
    : base(regex: @"^leave$")
    {
        _beingRepository = beingRepository;
        _state = state;
    }

    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasSelectedBeing
    ];

    protected override string Description =>
        "Moves the current being out of an inside room.";

    public override async Task<string> Invoke()
    {
        var thisBeing = await _state.GetBeing();

        // Check if outside room exists
        var thisRoom = await _state.GetRoom();
        // Find being whose inside room is this room
        var hostBeing = await _beingRepository
            .FindBeingByRoomInside(thisRoom);
        if(hostBeing is null)
        {
            return $"{thisBeing.Name} is not in inside room.";
        }

        // Move outside
        thisBeing.InRoom = hostBeing.InRoom;
        await _beingRepository.UpdateBeing(thisBeing);
        return $"{thisBeing.Name} moved to {thisBeing.InRoom.Name}.";
    }
}
