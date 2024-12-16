using System;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowItemsCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    protected override string Description =>
        "Shows all items";

    private readonly IGameResponse _response;
    private readonly IItemRepository _itemRepository;

    public ShowItemsCommand(
        IGameResponse response,
        IItemRepository itemRepository
    )
    : base(regex: @"^show items$")
    {
        _itemRepository = itemRepository;
        _response = response;
    }

    public override async Task Invoke()
    {
        var items = await _itemRepository.FindItems();
        if(items.Count == 0)
        {
            _response.AddText("There are no items.");
            return;
        }

        var itemNames = new List<string>();
        foreach(var item in items)
        {
            itemNames.Add(item.Name);
        }
        itemNames.Sort();

        _response.AddText(
            $"Items are: {MessageStandard.List(itemNames)}."
        );
    }
}
