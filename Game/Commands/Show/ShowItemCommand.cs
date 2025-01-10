using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;

namespace MUS.Game.Commands.Show;

public class ShowItemCommand : IGameCommand
{
    public bool AdminOnly => false;

    public string HelpText => "Shows details about an item.";

    public Regex Pattern => new("^(show|s) item (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 2);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public ShowItemCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run()
    {
        var item = await _context.FindItem(ItemNameInInput);
        if(item is null)
        {
            return CommandResult.ItemDoesNotExist(ItemNameInInput);
        }
        else
        {
            return item.Show();
        }
    }
}
