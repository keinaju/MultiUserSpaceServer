using System;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Set;

public class SetRoomPoolRequiredItemCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder,
        Prerequisite.UserHasSelectedBeing
    ];

    private readonly IGameResponse _response;
    private readonly IItemRepository _itemRepository;
    private readonly IRoomPoolRepository _roomPoolRepository;

    private string RoomPoolName => GetParameter(1);
    private string ItemName => GetParameter(2);

    protected override string Description =>
        "Sets an item that is required to explore a room pool.";

    public SetRoomPoolRequiredItemCommand(
        IGameResponse response,
        IItemRepository itemRepository,
        IRoomPoolRepository roomPoolRepository
    )
    : base(regex: @"^set room pool (.+) required item (.+)$")
    {
        _response = response;
        _itemRepository = itemRepository;
        _roomPoolRepository = roomPoolRepository;
    }

    public override async Task Invoke()
    {
        var roomPool = await _roomPoolRepository
            .FindRoomPool(RoomPoolName);
        if(roomPool is null)
        {
            _response.AddText(
                MessageStandard.DoesNotExist(
                    "Room pool", RoomPoolName
                )
            );
            return;
        }

        var item = await _itemRepository.FindItem(ItemName);
        if(item is null)
        {
            _response.AddText(
                MessageStandard.DoesNotExist(
                    "Item", ItemName
                )
            );
            return;
        }

        roomPool.ItemToExplore = item;
        await _roomPoolRepository.UpdateRoomPool(roomPool);

        _response.AddText(
            MessageStandard.Set(
                $"{roomPool.Name}'s required item", item.Name
            )
        );
    }
}
