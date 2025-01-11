using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Show;

public class ShowRoomPoolCommand : IUserCommand
{
    public bool AdminOnly => false;

    public string HelpText => "Shows details about a room pool.";

    public Regex Pattern => new("^(show|s) pool (.+)$");

    private string RoomPoolNameInInput => _input.GetGroup(this.Pattern, 2);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public ShowRoomPoolCommand(
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
            return RoomPoolDoesNotExist(RoomPoolNameInInput);
        }
        else
        {
            return pool.Show();
        }
    }
}
