using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.New;

public class NewItemHatcherCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasPickedBeing,
    ];

    private readonly IItemHatcherRepository _itemHatcherRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IPlayerState _state;

    private string ItemName => GetParameter(1);

    public NewItemHatcherCommand(
        IItemHatcherRepository itemHatcherRepository,
        IItemRepository itemRepository,
        IPlayerState state
    )
    : base(regex: @"^new (.+) hatcher$")
    {
        _itemHatcherRepository = itemHatcherRepository;
        _itemRepository = itemRepository;
        _state = state;
    }

    public override async Task<string> Invoke()
    {
        var item = await _itemRepository.FindItem(ItemName);
        if (item is null)
        {
            return MessageStandard.DoesNotExist("Item", ItemName);
        }

        var hatcher = new ItemHatcher()
        {
            Item = item,
            MinQuantity = 1,
            MaxQuantity = 1,
            IntervalInTicks = 1
        };
        var currentRoom = await _state.GetRoom();
        hatcher.Inventories.Add(currentRoom.Inventory);

        await _itemHatcherRepository.CreateItemHatcher(hatcher);

        return MessageStandard.Created("item hatcher", ItemName)
            + $" {currentRoom.Name} is subscribed to this.";
    }
}
