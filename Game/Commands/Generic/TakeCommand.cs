using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Generic;

public class TakeCommand : IUserCommand
{
    public bool AdminOnly => false;

    public string HelpText => "Takes items from the current room's inventory.";

    public Regex Pattern => new("^take (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public TakeCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        if(user.SelectedBeing is null)
        {
            return user.NoSelectedBeingResult();
        }
        else
        {
            var item = await _context.FindItem(ItemNameInInput);
            if(item is null)
            {
                return CommandResult.ItemDoesNotExist(ItemNameInInput);
            }
            else
            {
                return await user.SelectedBeing.TakeItemStackFromCurrentRoom(item);
            }
        }
    }
}
