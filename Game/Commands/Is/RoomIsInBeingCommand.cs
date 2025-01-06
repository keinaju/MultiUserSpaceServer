using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class RoomIsInBeingCommand : IGameCommand
{
    public string HelpText =>
    "Sets the current room as an inside room of the current being.";

    public Condition[] Conditions =>
    [
    ];

    public Regex Regex => new("^room is inside$");
    
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public RoomIsInBeingCommand(
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
            await RoomIsInBeing()
        );
    }

    private async Task<CommandResult> RoomIsInBeing()
    {
        if(_session.AuthenticatedUser is not null)
        {
            return await _session.AuthenticatedUser
            .RoomIsInBeing();
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
