using System;
using MUS.Game.Data;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Set;

public class SetRoomGlobalAccessCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder,
        Prerequisite.UserHasSelectedBeing
    ];

    protected override string Description =>
    "Sets a setting for the current room to determine if it can be entered from anywhere.";

    private string TrueOrFalseInUserInput => GetParameter(1);

    private const string TRUE_PARAMETER = "true"; 
    private const string FALSE_PARAMETER = "false";

    private readonly IGameResponse _response;
    private readonly IPlayerState _state;
    private readonly IRoomRepository _roomRepository;

    public SetRoomGlobalAccessCommand(
        IGameResponse response,
        IPlayerState state,
        IRoomRepository roomRepository
    )
    : base(regex: $"^set room global access ({TRUE_PARAMETER}|{FALSE_PARAMETER})$")
    {
        _response = response;
        _roomRepository = roomRepository;
        _state = state;
    }

    public override async Task Invoke()
    {
        var room = await _state.GetRoom();
        
        if(TrueOrFalseInUserInput == TRUE_PARAMETER)
        {
            room.GlobalAccess = true;
        }
        else if(TrueOrFalseInUserInput == FALSE_PARAMETER)
        {
            room.GlobalAccess = false;
        }

        await _roomRepository.UpdateRoom(room);

        _response.AddText(
            MessageStandard.Set(
                $"{room.Name}'s global accessibility",
                TrueOrFalseInUserInput
            )
        );
    }
}
