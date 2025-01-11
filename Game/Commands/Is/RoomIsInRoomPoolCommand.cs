using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Is;

public class RoomIsInRoomPoolCommand : IUserCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Adds the current room in a room pool.";

    public Regex Pattern => new("^room is in pool (.+)$");

    private string RoomPoolNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public RoomIsInRoomPoolCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        var pool = await _context.FindRoomPool(RoomPoolNameInInput);
        if(pool is null)
        {
            return CommandResult.RoomPoolDoesNotExist(RoomPoolNameInInput);
        }
        else
        {
            return await user.RoomIsInRoomPool(pool);
        }
    }
}
