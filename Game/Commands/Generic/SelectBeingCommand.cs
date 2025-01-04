using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Generic;

public class SelectBeingCommand : IGameCommand
{
    public Condition[] Conditions =>
    [
    ];

    public string HelpText => "Selects a being to use.";

    public Regex Regex => new("^select (.+)$");

    private string BeingNameInInput =>
    _input.GetGroup(this.Regex, 1);

    private readonly IResponsePayload _response;
    private readonly ISessionService _session;
    private readonly IInputCommand _input;

    public SelectBeingCommand(
        IResponsePayload response,
        ISessionService session,
        IInputCommand input
    )
    {
        _response = response;
        _session = session;
        _input = input;
    }

    public async Task Run()
    {
        _response.AddResult(
            await Select()
        );
    }

    private async Task<CommandResult> Select()
    {
        if(_session.AuthenticatedUser is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }
        else
        {
            return await _session.AuthenticatedUser
            .SelectBeing(BeingNameInInput);
        }
    }
}
