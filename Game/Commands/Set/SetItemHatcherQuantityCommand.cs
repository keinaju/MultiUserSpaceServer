using System;
using MUS.Game.Data;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Set;

public class SetItemHatcherQuantityCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder,
        Prerequisite.UserHasSelectedBeing
    ];

    private readonly IGameResponse _response;
    private readonly IItemHatcherRepository _itemHatcherRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IPlayerState _state;
    
    private string ItemName => GetParameter(1);
    private string MinInUserInput => GetParameter(2);
    private string MaxInUserInput => GetParameter(3);

    protected override string Description =>
    "Sets a minimum and maximum quantity of items to generate for an item hatcher.";

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
        IGameResponse response,
        IItemHatcherRepository itemHatcherRepository,
        IItemRepository itemRepository,
        IPlayerState state
    )
    : base(regex: @"^set (.+) item hatcher quantity (\d+) to (\d+)$")
    {
        _itemHatcherRepository = itemHatcherRepository;
        _itemRepository = itemRepository;
        _response = response;
        _state = state;
    }

    public override async Task Invoke()
    {
        var item = await _itemRepository.FindItem(ItemName);
        if(item is null)
        {
            _response.AddText(
                MessageStandard.DoesNotExist("Item", ItemName)
            );
            return;
        }

        var min = ParsedQuantity(MinInUserInput);
        if(min is null)
        {
            _response.AddText(
                MessageStandard.Invalid(MinInUserInput, "minimum quantity")
            );
            return;
        }

        var max = ParsedQuantity(MaxInUserInput);
        if(max is null)
        {
            _response.AddText(
                MessageStandard.Invalid(MaxInUserInput, "maximum quantity")
            );
            return;
        }

        if(max < min)
        {
            _response.AddText(
                "Maximum quantity can not be smaller than minimum quantity."
            );
            return;
        }

        var room = await _state.GetRoom();

        foreach(var hatcher in room.Inventory.ItemHatchers)
        {
            if(hatcher.Item.PrimaryKey == item.PrimaryKey)
            {
                hatcher.MinQuantity = (int)min;
                hatcher.MaxQuantity = (int)max;
                await _itemHatcherRepository.UpdateItemHatcher(hatcher);
                _response.AddText(
                    MessageStandard.Set(
                        "Item hatcher", $"({hatcher.GetDetails()})"
                    )
                );
                return; 
            }
        }

        _response.AddText(
            MessageStandard.DoesNotContain(
                $"{room.Name}", $"a hatcher for {item.Name}"
            )
        );
    }
}
