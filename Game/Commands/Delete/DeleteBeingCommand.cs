using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Delete;

public class DeleteBeingCommand : IGameCommand
{
    public string HelpText => "Deletes a being.";

    public Condition[] Conditions =>
    [
    ];

    public Regex Regex => new("^delete being (.+)$");

    private string BeingNameInInput => _input.GetGroup(this.Regex, 1);

    private readonly IResponsePayload _response;
    private readonly ISessionService _session;
    private readonly IInputCommand _input;

    public DeleteBeingCommand(
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
            await DeleteBeing()
        );
    }

    private async Task<CommandResult> DeleteBeing()
    {
        if(_session.AuthenticatedUser is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }
        else
        {
            return await _session.AuthenticatedUser
            .DeleteBeing(BeingNameInInput);
        }
    }
}
