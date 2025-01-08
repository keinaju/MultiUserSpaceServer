using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.New;

public class NewBeingCommand : IGameCommand
{
    public string HelpText => "Creates a new being.";

    public Regex Regex => new("^new being (.+)$");

    public Condition[] Conditions =>
    [
    ];

    private string BeingNameInInput => _input.GetGroup(this.Regex, 1);

    private readonly IInputCommand _input;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public NewBeingCommand(
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
            await NewBeing()
        );
    }

    private async Task<CommandResult> NewBeing()
    {
        if(_session.AuthenticatedUser is not null)
        {
            return await _session.AuthenticatedUser.NewBeing(BeingNameInInput);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
