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
    private readonly IGameResponse _response;
    private readonly IPlayerState _state;
    private readonly IRoomRepository _roomRepository;

    private string RoomName => GetParameter(1);

    protected override string Description =>
    "Sets a room that resides inside the current being.";

    public SetRoomInsideBeingCommand(
        IBeingRepository beingRepository,
        IGameResponse response,
        IPlayerState state,
        IRoomRepository roomRepository
    )
    : base(regex: @"^set inside (.+)$")
    {
        _beingRepository = beingRepository;
        _response = response;
        _roomRepository = roomRepository;
        _state = state;
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

        var being = await _state.GetBeing();
        being.RoomInside = room;
        await _beingRepository.UpdateBeing(being);

        _response.AddText(
            MessageStandard.Set(
                $"{being.Name}'s inside room",
                RoomName
            )
        );
    }
}
