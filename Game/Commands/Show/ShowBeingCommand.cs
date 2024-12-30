using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Show;

public class ShowBeingCommand : IGameCommand
{
    public string HelpText =>
    "Shows the currently selected being.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserHasSelectedBeing
    ];

    public Regex Regex => new("^(show|s) being$");

    private Being SelectedBeing => _player.GetSelectedBeing();

    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;

    public ShowBeingCommand(
        IPlayerState player,
        IResponsePayload response
    )
    {
        _player = player;
        _response = response;
    }

    public Task Run()
    {
        _response.AddList(SelectedBeing.Show());

        return Task.CompletedTask;
    }
}
