using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Show;

public class ShowItemsCommand : IUserCommand
{
    public bool AdminOnly => false;

    public string HelpText => "Shows all items.";

    public Regex Pattern => new("^(show|s) items$");

    private readonly GameContext _context;

    public ShowItemsCommand(GameContext context)
    {
        _context = context;
    }

    public async Task<CommandResult> Run(User user)
    {
        var items = await _context.FindAllItems();

        if(items.Count == 0)
        {
            return new CommandResult(StatusCode.Success)
            .AddMessage("There are no items.");
        }
        else
        {
            return new CommandResult(StatusCode.Success)
            .AddMessage($"All items are: {GetItemNames(items)}.");
        }
    }

    private string GetItemNames(IEnumerable<Item> items)
    {
        var itemNames = new List<string>();

        foreach(var item in items)
        {
            itemNames.Add(item.Name);
        }

        itemNames.Sort();

        return Message.List(itemNames);
    }
}
