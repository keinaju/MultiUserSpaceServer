using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class RoomIsInRoomPoolCommand : IGameCommand
{
    public bool AdminOnly => true;

    public string HelpText =>
    "Adds the current room in a room pool.";

    public Regex Pattern => new("^room is in pool (.+)$");

    private string RoomPoolNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly GameContext _context;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public RoomIsInRoomPoolCommand(
        GameContext context,
        IInputCommand input,
        ISessionService session
    )
    {
        _context = context;
        _input = input;
        _session = session;
    }

    public async Task<CommandResult> Run()
    {
        var pool = await _context.FindRoomPool(RoomPoolNameInInput);
        if(pool is null)
        {
            return CommandResult.RoomPoolDoesNotExist(RoomPoolNameInInput);
        }

        if(_session.User is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }
        else
        {
            return await _session.User.RoomIsInRoomPool(pool);
        }
    }
}
