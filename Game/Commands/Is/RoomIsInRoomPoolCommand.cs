using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class RoomIsInRoomPoolCommand : IGameCommand
{
    public string HelpText =>
    "Adds the current room in a room pool.";

    public Condition[] Conditions =>
    [
    ];

    public Regex Regex => new("^room is in pool (.+)$");

    private string RoomPoolNameInInput =>
    _input.GetGroup(this.Regex, 1);


    private readonly GameContext _context;
    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public RoomIsInRoomPoolCommand(
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
            await RoomIsInRoomPool()
        );
    }

    private async Task<CommandResult> RoomIsInRoomPool()
    {
        var pool = await _context
        .FindRoomPool(RoomPoolNameInInput);
        if(pool is null)
        {
            return CommandResult
            .RoomPoolDoesNotExist(RoomPoolNameInInput);
        }

        if(_session.AuthenticatedUser is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }
        else
        {
            return await _session.AuthenticatedUser
            .RoomIsInRoomPool(pool);
        }
    }
}
