using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Generic;

public class BreakCommand : ICommandPattern
{
    public bool AdminOnly => false;
    
    public string HelpText => "Breaks an item to it's components.";

    public Regex Pattern => new("^break (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;

    public BreakCommand(IInputCommand input)
    {
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
            return await user.SelectedBeing.TryBreakItem(ItemNameInInput);
        }
    }
}
