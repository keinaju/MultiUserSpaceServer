using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowItemsCommand : IGameCommand
{
    public string HelpText => "Shows all items.";

    public Condition[] Conditions => [];

    public Regex Regex => new("^(show|s) items$");

    private readonly IItemRepository _itemRepo;
    private readonly IResponsePayload _response;

    public ShowItemsCommand(
        IItemRepository itemRepo,
        IResponsePayload response
    )
    {
        _itemRepo = itemRepo;
        _response = response;
    }

    public async Task Run()
    {
        var items = await _itemRepo.FindItems();
        if(items.Count == 0)
        {
            _response.AddText("There are no items.");
            return;
        }

        _response.AddText($"All items are: {GetItemNames(items)}.");
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
