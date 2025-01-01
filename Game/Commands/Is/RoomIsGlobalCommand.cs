using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Is;

public class RoomIsGlobalCommand : IGameCommand
{
    public string HelpText =>
    "Sets the global accessibility of the current room.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder,
        Condition.UserHasSelectedBeing
    ];

    public Regex Regex => new("^room (is|is not) global$");

    private string IsOrIsNotInInput =>
    _input.GetGroup(this.Regex, 1);

    private bool TrueOrFalse =>
    IsOrIsNotInInput == "is" ? true : false;

    private Room CurrentRoom => _player.GetCurrentRoom();

    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IRoomRepository _roomRepo;
    private readonly IInputCommand _input;

    public RoomIsGlobalCommand(
        IPlayerState player,
        IResponsePayload response,
        IRoomRepository roomRepo,
        IInputCommand input
    )
    {
        _player = player;
        _response = response;        
        _roomRepo = roomRepo;        
        _input = input;        
    }

    public async Task Run()
    {
        await SetRoomGlobalAccess();

        _response.AddText(
            Message.Set(
                $"{CurrentRoom.Name}'s global access",
                TrueOrFalse.ToString()
            )
        );
    }

    private async Task SetRoomGlobalAccess()
    {
        CurrentRoom.GlobalAccess = TrueOrFalse;

        await _roomRepo.UpdateRoom(CurrentRoom);
    }
}
