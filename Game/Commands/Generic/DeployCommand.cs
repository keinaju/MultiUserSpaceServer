using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Generic;

public class DeployCommand : IGameCommand
{
    public string HelpText => "Deploys an item to a being.";

    public Regex Regex => new("^deploy (.+)$");

    private string ItemNameInInput =>
    _input.GetGroup(this.Regex, 1);

    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public DeployCommand(
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
            await Deploy()
        );
    }

    private async Task<CommandResult> Deploy()
    {
        if(_session.AuthenticatedUser is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }
        else
        {
            return await _session.AuthenticatedUser
            .DeployItem(ItemNameInInput);
        }
    }
}
