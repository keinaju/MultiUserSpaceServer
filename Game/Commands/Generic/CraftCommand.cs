using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Generic;

public class CraftCommand : ICommandPattern
{
    public bool AdminOnly => false;
    
    public string HelpText => "Crafts an item from components.";

    public Regex Pattern => new("^craft (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;

    public CraftCommand(IInputCommand input)
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
            return await user.SelectedBeing.TryCraftItem(ItemNameInInput);
        }
    }
}
