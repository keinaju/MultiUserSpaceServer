using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Generic;

public class GoCommand : IGameCommand
{
    public string HelpText =>
    "Moves a selected being to another room.";

    public Regex Regex => new("^go (.+)$");

    private string RoomNameInInput =>
    _input.GetGroup(this.Regex, 1);

    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public GoCommand(
        IResponsePayload response,
        IInputCommand input,
        ISessionService session
    )
    {
        _response = response;
        _input = input;
        _session = session;
    }

    public async Task Run()
    {
        _response.AddResult(
            await Go()
        );
    }

    private async Task<CommandResult> Go()
    {
        if(_session.AuthenticatedUser is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }
        else
        {
            return await _session.AuthenticatedUser.Go(
                RoomNameInInput
            );
        }
    }
}
