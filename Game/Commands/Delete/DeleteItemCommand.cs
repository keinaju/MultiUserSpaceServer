using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Delete;

public class DeleteItemCommand : IUserCommand
{
    public bool AdminOnly => true;
    
    public string HelpText => "Deletes an item.";

    public Regex Pattern => new("^delete item (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public DeleteItemCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        return await _context.DeleteItem(ItemNameInInput);
    }
}
