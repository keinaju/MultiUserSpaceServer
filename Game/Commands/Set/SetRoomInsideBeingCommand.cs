using System;
using MUS.Game.Data;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Set;

public class SetRoomInsideBeingCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder,
        Prerequisite.UserHasSelectedBeing
    ];

    private readonly IBeingRepository _beingRepository;
    private readonly IPlayerState _state;
    private readonly IRoomRepository _roomRepository;

    private string RoomName => GetParameter(1);

    protected override string Description =>
        "Sets a room that resides inside the current being.";

    public SetRoomInsideBeingCommand(
        IBeingRepository beingRepository,
        IPlayerState state,
        IRoomRepository roomRepository
    )
    : base(regex: @"^set inside (.+)$")
    {
        _beingRepository = beingRepository;
        _roomRepository = roomRepository;
        _state = state;
    }

    public override async Task<string> Invoke()
    {
        var room = await _roomRepository.FindRoom(RoomName);
        if(room is null)
        {
            return MessageStandard.DoesNotExist("Room", RoomName);
        }

        var being = await _state.GetBeing();
        being.RoomInside = room;
        await _beingRepository.UpdateBeing(being);

        return MessageStandard.Set($"{being.Name}'s inside room", RoomName);
    }
}
