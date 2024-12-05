using System;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowItemsCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    protected override string Description =>
        "Shows all items";

    private readonly IItemRepository _itemRepository;

    public ShowItemsCommand(IItemRepository itemRepository)
    : base(regex: @"^show items$")
    {
        _itemRepository = itemRepository;
    }

    public override async Task<string> Invoke()
    {
        var items = await _itemRepository.FindItems();
        if(items.Count == 0)
        {
            return "There are no items.";
        }

        var itemNames = new List<string>();
        foreach(var item in items)
        {
            itemNames.Add(item.Name);
        }
        itemNames.Sort();

        return $"Items are: {MessageStandard.List(itemNames)}.";
    }
}
