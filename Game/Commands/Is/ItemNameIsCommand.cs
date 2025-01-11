using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Is;

public class ItemNameIsCommand : IUserCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Renames an item.";

    public Regex Pattern => new("^item (.+) name is (.+)$");

    private string OldItemNameInInput => _input.GetGroup(this.Pattern, 1);

    private string NewItemNameInInput => _input.GetGroup(this.Pattern, 2);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public ItemNameIsCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        var item = await _context.FindItem(OldItemNameInInput);
        if(item is null)
        {
            return ItemDoesNotExist(OldItemNameInInput);
        }
        else
        {
            return await user.ItemNameIs(item, NewItemNameInInput);
        }
    }
}
