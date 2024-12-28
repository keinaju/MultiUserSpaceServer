using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;

namespace MUS.Game.Commands.Show;

public class ShowRoomCommand : IGameCommand
{
    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserHasSelectedBeing
    ];
    
    public string HelpText => "Shows the current room.";

    public Regex Regex => new("^(look|show room)$");

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
        _response.AddList(
            _player.GetCurrentRoom().Show()
        );

        return Task.CompletedTask;
    }
}
