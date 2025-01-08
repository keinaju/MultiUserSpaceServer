using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Show;

public class ShowRoomCommand : IGameCommand
{
    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserHasSelectedBeing
    ];
    
    public string HelpText => "Shows the current room.";

    public Regex Regex => new("^(show|s) (room|r)$");

    private Being CurrentBeing => _player.GetSelectedBeing();

    private Room CurrentRoom => _player.GetCurrentRoom();

    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;

    public ShowRoomCommand(
        IPlayerState player,
        IResponsePayload response
    )
    {
        _player = player;
        _response = response;
    }

    public Task Run()
    {
        _response.AddText(
            $"{CurrentBeing.Name} looks at the {CurrentRoom.Name}."
        );

        _response.AddList(
            _player.GetCurrentRoom().GetDetails()
        );

        return Task.CompletedTask;
    }
}
