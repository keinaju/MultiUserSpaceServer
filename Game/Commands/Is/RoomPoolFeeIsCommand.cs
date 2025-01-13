using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Is;

public class RoomPoolFeeIsCommand : ICommandPattern
{
    public bool AdminOnly => true;

    public string HelpText => "Sets the fee item of a room pool.";

    public Regex Pattern => new("^pool (.+) fee is (.+)$");

    private string RoomPoolNameInInput => _input.GetGroup(this.Pattern, 1);

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 2);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public RoomPoolFeeIsCommand(
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
        
        var item = await _context.FindItem(ItemNameInInput);
        if(item is null)
        {
            return CommandResult.ItemDoesNotExist(ItemNameInInput);
        }

        return await pool.SetFeeItem(item);
    }
}
