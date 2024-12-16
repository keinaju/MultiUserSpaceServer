using System;
using MUS.Game.Data;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Commands;

public class LeaveCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasSelectedBeing
    ];

    protected override string Description =>
        "Moves the current being out of an inside room.";

    private readonly IBeingRepository _beingRepository;
    private readonly IGameResponse _response;
    private readonly IPlayerState _state;

    public LeaveCommand(
        IBeingRepository beingRepository,
        IGameResponse response,
        IPlayerState state
    )
    : base(regex: @"^leave$")
    {
        _beingRepository = beingRepository;
        _response = response;
        _state = state;
    }

    public override async Task Invoke()
    {
        var thisBeing = await _state.GetBeing();

        // Check if outside room exists
        var thisRoom = await _state.GetRoom();
        // Find being whose inside room is this room
        var hostBeing = await _beingRepository
        .FindBeingByRoomInside(thisRoom);
        if(hostBeing is null)
        {
            _response.AddText($"{thisBeing.Name} is not in inside room.");
            return;
        }

        // Move outside
        thisBeing.InRoom = hostBeing.InRoom;
        await _beingRepository.UpdateBeing(thisBeing);
        _response.AddText(
            $"{thisBeing.Name} moved to {thisBeing.InRoom.Name}."
        );
    }
}
