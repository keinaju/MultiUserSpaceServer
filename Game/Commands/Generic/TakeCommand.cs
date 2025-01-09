using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Generic;

public class TakeCommand : IGameCommand
{
    public string HelpText => "Takes items from the current room's inventory.";

    public Regex Regex => new("^take (.+)$");

    private string ItemNameInInput =>
    _input.GetGroup(this.Regex, 1);

    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public TakeCommand(
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
            await Take()
        );

    }

    private async Task<CommandResult> Take()
    {
        if(_session.AuthenticatedUser is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }
        else
        {
            return await _session.AuthenticatedUser
            .TakeItem(ItemNameInInput);
        }
    }
}
