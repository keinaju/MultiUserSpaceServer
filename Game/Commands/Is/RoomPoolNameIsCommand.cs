using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class RoomPoolNameIsCommand : IGameCommand
{
    public string HelpText => "Renames a room pool.";

    public Condition[] Conditions =>
    [
    ];

    public Regex Regex => new("^pool (.+) name is (.+)$");

    private string OldRoomPoolNameInInput =>
    _input.GetGroup(this.Regex, 1);

    private string NewRoomPoolNameInInput =>
    _input.GetGroup(this.Regex, 2);

    private readonly GameContext _context;
    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public RoomPoolNameIsCommand(
        GameContext context,
        IResponsePayload response,
        IInputCommand input,
        ISessionService session
    )
    {
        _context = context;
        _response = response;
        _input = input;
        _session = session;
    }

    public async Task Run()
    {
        _response.AddResult(
            await RoomPoolNameIs()
        );
    }

    private async Task<CommandResult> RoomPoolNameIs()
    {
        var pool = await _context.FindRoomPool(OldRoomPoolNameInInput);
        if(pool is null)
        {
            return CommandResult.RoomPoolDoesNotExist(OldRoomPoolNameInInput);
        }

        if(_session.AuthenticatedUser is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }

        return await _session.AuthenticatedUser
        .RoomPoolNameIs(pool, NewRoomPoolNameInInput);
    }
}
