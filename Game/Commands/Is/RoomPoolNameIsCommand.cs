using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class RoomPoolNameIsCommand : IGameCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Renames a room pool.";

    public Regex Pattern => new("^pool (.+) name is (.+)$");

    private string OldRoomPoolNameInInput => _input.GetGroup(this.Pattern, 1);

    private string NewRoomPoolNameInInput => _input.GetGroup(this.Pattern, 2);

    private readonly GameContext _context;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public RoomPoolNameIsCommand(
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
        var pool = await _context.FindRoomPool(OldRoomPoolNameInInput);
        if(pool is null)
        {
            return CommandResult.RoomPoolDoesNotExist(OldRoomPoolNameInInput);
        }

        if(_session.User is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }
        else
        {
            return await _session.User.RoomPoolNameIs(
                pool,
                NewRoomPoolNameInInput
            );
        }
    }
}
