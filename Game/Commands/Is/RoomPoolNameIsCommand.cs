using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Is;

public class RoomPoolNameIsCommand : IUserCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Renames a room pool.";

    public Regex Pattern => new("^pool (.+) name is (.+)$");

    private string OldRoomPoolNameInInput => _input.GetGroup(this.Pattern, 1);

    private string NewRoomPoolNameInInput => _input.GetGroup(this.Pattern, 2);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public RoomPoolNameIsCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        var pool = await _context.FindRoomPool(OldRoomPoolNameInInput);
        if(pool is null)
        {
            return CommandResult.RoomPoolDoesNotExist(OldRoomPoolNameInInput);
        }
        else
        {
            return await user.RoomPoolNameIs(
                pool,
                NewRoomPoolNameInInput
            );
        }
    }
}
