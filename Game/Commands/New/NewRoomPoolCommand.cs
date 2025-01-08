using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.New;

public class NewRoomPoolCommand : IGameCommand
{
    public string HelpText => "Creates a new room pool.";

    public Condition[] Conditions =>
    [
    ];

    public Regex Regex => new("^new pool (.+)$");

    private string PoolNameInInput => _input.GetGroup(this.Regex, 1);

    private readonly IInputCommand _input;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public NewRoomPoolCommand(
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
            await NewRoomPool()
        );
    }

    private async Task<CommandResult> NewRoomPool()
    {
        if(_session.AuthenticatedUser is not null)
        {
            return await _session.AuthenticatedUser
            .NewRoomPool(PoolNameInInput);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
