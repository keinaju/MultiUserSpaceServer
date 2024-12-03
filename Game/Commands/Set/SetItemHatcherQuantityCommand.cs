using System;
using MUS.Game.Data;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Set;

public class SetItemHatcherQuantityCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasPickedBeing
    ];

    private readonly IItemHatcherRepository _itemHatcherRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IPlayerState _state;
    
    private string ItemName => GetParameter(1);
    private string MinInUserInput => GetParameter(2);
    private string MaxInUserInput => GetParameter(3);

    private int? ParsedQuantity(string input)
    {
        if(int.TryParse(input, out int parsed))
        {
            if (parsed <= 0) return null;

            return parsed;
        }

        return null;
    }

    public SetItemHatcherQuantityCommand(
        IItemHatcherRepository itemHatcherRepository,
        IItemRepository itemRepository,
        IPlayerState state
    )
    : base(regex: @"^set (.+) item hatcher quantity (\d+) to (\d+)$")
    {
        _itemHatcherRepository = itemHatcherRepository;
        _itemRepository = itemRepository;
        _state = state;
    }

    public override async Task<string> Invoke()
    {
        var item = await _itemRepository.FindItem(ItemName);
        if(item is null)
        {
            return MessageStandard.DoesNotExist("Item", ItemName);
        }

        var min = ParsedQuantity(MinInUserInput);
        if(min is null)
        {
            return MessageStandard.Invalid(MinInUserInput, "minimum quantity");
        }

        var max = ParsedQuantity(MaxInUserInput);
        if(max is null)
        {
            return MessageStandard.Invalid(MaxInUserInput, "maximum quantity");
        }

        if(max < min)
        {
            return "Maximum quantity can not be smaller than minimum quantity.";
        }

        var room = await _state.GetRoom();

        foreach(var hatcher in room.Inventory.ItemHatchers)
        {
            if(hatcher.Item.PrimaryKey == item.PrimaryKey)
            {
                hatcher.MinQuantity = (int)min;
                hatcher.MaxQuantity = (int)max;
                await _itemHatcherRepository.UpdateItemHatcher(hatcher);
                return MessageStandard.Set(
                    "Item hatcher", $"({hatcher.GetDetails()})"
                );
            }
        }

        return MessageStandard.DoesNotContain(
            $"{room.Name}", $"a hatcher for {item.Name}"
        );
    }
}
