using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Generic;

public class BreakCommand : IGameCommand
{
    public string HelpText => "Breaks an item to components.";

    public Condition[] Conditions =>
    [
    ];

    public Regex Regex => new("^break (.+)$");

    private string ItemNameInInput =>
    _input.GetGroup(this.Regex, 1);

    private readonly IInputCommand _input;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public BreakCommand(
        IInputCommand input,
        IResponsePayload response,
        ISessionService session
    )
    {
        _input = input;
        _response = response;
        _session = session;
    }

    public async Task Run()
    {
        _response.AddResult(
            await BreakItem()
        );
    }

    private async Task<CommandResult> BreakItem()
    {
        if(_session.AuthenticatedUser is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }
        else
        {
            return await _session.AuthenticatedUser
            .BreakItem(ItemNameInInput);
        }
    }
}
