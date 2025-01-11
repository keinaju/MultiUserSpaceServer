using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Is;

public class ItemDescriptionIsCommand : IUserCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Sets the description of an item.";

    public Regex Pattern => new("^item (.+) description is (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 1);
    
    private string DescriptionInInput => _input.GetGroup(this.Pattern, 2);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public ItemDescriptionIsCommand(
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
        else
        {
            return await item.SetDescription(DescriptionInInput);
        }
    }
}
