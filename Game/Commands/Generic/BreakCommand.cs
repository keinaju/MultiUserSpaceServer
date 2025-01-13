using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Generic;

public class BreakCommand : ICommandPattern
{
    public bool AdminOnly => false;
    
    public string HelpText => "Breaks an item to it's components.";

    public Regex Pattern => new("^break (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public BreakCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        var item = await _context.FindItem(ItemNameInInput);
        if(item is null)
        {
            return CommandResult.ItemDoesNotExist(ItemNameInInput);
        }

        if(user.SelectedBeing is null)
        {
            return user.NoSelectedBeingResult();
        }

        return await user.SelectedBeing.TryBreakItem(item);
    }
}
