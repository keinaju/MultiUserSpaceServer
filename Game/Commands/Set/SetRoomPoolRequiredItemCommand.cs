using System;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Set;

public class SetRoomPoolRequiredItemCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasPickedBeing
    ];

    private readonly IItemRepository _itemRepository;
    private readonly IRoomPoolRepository _roomPoolRepository;

    private string RoomPoolName => GetParameter(1);
    private string ItemName => GetParameter(2);

    public SetRoomPoolRequiredItemCommand(
        IItemRepository itemRepository,
        IRoomPoolRepository roomPoolRepository
    )
    : base(regex: @"^set room pool (.+) required item (.+)$")
    {
        _itemRepository = itemRepository;
        _roomPoolRepository = roomPoolRepository;
    }

    public override async Task<string> Invoke()
    {
        var roomPool = await _roomPoolRepository
            .FindRoomPool(RoomPoolName);
        if(roomPool is null)
        {
            return MessageStandard.DoesNotExist("Room pool", RoomPoolName);
        }

        var item = await _itemRepository.FindItem(ItemName);
        if(item is null)
        {
            return MessageStandard.DoesNotExist("Item", ItemName);
        }

        roomPool.ItemToExplore = item;
        await _roomPoolRepository.UpdateRoomPool(roomPool);

        return MessageStandard.Set(
            $"{roomPool.Name}'s required item", item.Name
        );
    }
}
