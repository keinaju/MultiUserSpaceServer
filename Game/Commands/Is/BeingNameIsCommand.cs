using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class BeingNameIsCommand : IGameCommand
{
    public string HelpText =>
    "Renames the currently selected being.";

    public Condition[] Conditions =>
    [
    ];

    public Regex Regex => new("^being name is (.+)$");

    private string BeingNameInInput => _input.GetGroup(this.Regex, 1);

    private readonly IInputCommand _input;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public BeingNameIsCommand(
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
            await BeingNameIs()
        );
    }

    private async Task<CommandResult> BeingNameIs()
    {
        if(_session.AuthenticatedUser is not null)
        {
            return await _session.AuthenticatedUser
            .SelectedBeingNameIs(BeingNameInInput);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
