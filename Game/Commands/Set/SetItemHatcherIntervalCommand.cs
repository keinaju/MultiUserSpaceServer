using System;
using MUS.Game.Data;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Set;

public class SetItemHatcherIntervalCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder,
        Prerequisite.UserHasSelectedBeing
    ];

    private readonly IItemHatcherRepository _itemHatcherRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IPlayerState _state;

    private string ItemName => GetParameter(1);
    private string IntervalInUserInput => GetParameter(2);

    private int? ParsedInterval
    {
        get
        {
            if(int.TryParse(IntervalInUserInput, out int parsed))
            {
                if (parsed <= 0) return null;

                return parsed;
            }

            return null;
        }
    }

    protected override string Description =>
        "Sets a tick interval for an item hatcher.";

    public SetItemHatcherIntervalCommand(
        IItemHatcherRepository itemHatcherRepository,
        IItemRepository itemRepository,
        IPlayerState state
    )
    : base(regex: @"^set (.+) item hatcher interval (\d+)$")
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

        if(ParsedInterval is null)
        {
            return MessageStandard.Invalid(IntervalInUserInput, "interval");
        }

        var room = await _state.GetRoom();

        foreach(var hatcher in room.Inventory.ItemHatchers)
        {
            if(hatcher.Item.PrimaryKey == item.PrimaryKey)
            {
                hatcher.IntervalInTicks = (int)ParsedInterval;
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
