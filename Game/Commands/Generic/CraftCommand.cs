using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Generic;

public class CraftCommand : IGameCommand
{
    public string HelpText => "Crafts an item from components.";

    public Regex Regex => new("^craft (.+)$");

    private string ItemNameInInput =>
    _input.GetGroup(this.Regex, 1);

    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public CraftCommand(
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
            await CraftItem()
        );
    }

    private async Task<CommandResult> CraftItem()
    {
        if(_session.AuthenticatedUser is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }
        else
        {
            return await _session.AuthenticatedUser
            .CraftItem(ItemNameInInput);
        }
    }
}
