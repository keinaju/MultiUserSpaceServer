using System;
using MUS.Game.Data;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Set;

public class SetRoomGlobalAccessibilityCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasPickedBeing
    ];

    private readonly IPlayerState _state;
    private readonly IRoomRepository _roomRepository;
    private string TrueOrFalseInUserInput => GetParameter(1);

    public SetRoomGlobalAccessibilityCommand(
        IPlayerState state,
        IRoomRepository roomRepository
    )
    : base(regex: @"^set room global accessibility (true|false)$")
    {
        _roomRepository = roomRepository;
        _state = state;
    }

    public override async Task<string> Invoke()
    {
        var room = await _state.GetRoom();
        
        if(TrueOrFalseInUserInput == "true")
        {
            room.GlobalAccessibility = true;
        }
        else if(TrueOrFalseInUserInput == "false")
        {
            room.GlobalAccessibility = false;
        }

        await _roomRepository.UpdateRoom(room);

        return MessageStandard.Set(
            $"{room.Name}'s global accessibility",
            TrueOrFalseInUserInput
        );
    }
}
