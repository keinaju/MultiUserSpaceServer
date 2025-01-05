using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class CuriosityIsCommand : IGameCommand
{
    public string HelpText => "Sets the curiosity of the current room.";

    public Condition[] Conditions =>
    [
    ];

    public Regex Regex => new("^curiosity is (.+)$");

    private string RoomPoolNameInInput =>
    _input.GetGroup(this.Regex, 1);

    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public CuriosityIsCommand(
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
            await CuriosityIs()
        );
    }

    private async Task<CommandResult> CuriosityIs()
    {
        if(_session.AuthenticatedUser is not null)
        {
            return await _session.AuthenticatedUser
            .CuriosityIs(RoomPoolNameInInput);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
