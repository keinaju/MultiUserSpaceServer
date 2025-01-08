using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Show;

public class ShowBeingCommand : IGameCommand
{
    public string HelpText =>
    "Shows the currently selected being.";

    public Condition[] Conditions =>
    [
    ];

    public Regex Regex => new("^(show|s) being$");

    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public ShowBeingCommand(
        IResponsePayload response,
        ISessionService session
    )
    {
        _response = response;
        _session = session;
    }

    public async Task Run()
    {
        _response.AddResult(
            await ShowBeing()
        );
    }

    private async Task<CommandResult> ShowBeing()
    {
        if(_session.AuthenticatedUser is not null)
        {
            return await _session.AuthenticatedUser.ShowBeing();
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
