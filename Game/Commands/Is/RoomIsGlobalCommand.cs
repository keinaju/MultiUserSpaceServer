using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class RoomIsGlobalCommand : IGameCommand
{
    public string HelpText =>
    "Sets the global accessibility of the current room.";

    public Condition[] Conditions =>
    [
    ];

    public Regex Regex => new("^room (is|is not) global$");

    private string WordInInput => _input.GetGroup(this.Regex, 1);

    private bool NewValue => WordInInput == "is" ? true : false;

    private readonly IInputCommand _input;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public RoomIsGlobalCommand(
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
            await RoomIsGlobal()
        );
    }

    private async Task<CommandResult> RoomIsGlobal()
    {
        if(_session.AuthenticatedUser is not null)
        {
            return await _session.AuthenticatedUser
            .RoomIsGlobal(NewValue);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
