using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.New;

public class NewItemHatcherCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder,
        Prerequisite.UserHasPickedBeing,
    ];

    private readonly IItemHatcherRepository _itemHatcherRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IPlayerState _state;

    private string ItemName => GetParameter(1);

    protected override string Description =>
        "Creates a new item hatcher that generates items into inventories.";

    public NewItemHatcherCommand(
        IItemHatcherRepository itemHatcherRepository,
        IItemRepository itemRepository,
        IPlayerState state
    )
    : base(regex: @"^new (.+) item hatcher$")
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

        var room = await _state.GetRoom();
        // Do not allow multiple hatchers of one item in one inventory
        foreach(var hatcherInRoom in room.Inventory.ItemHatchers)
        {
            if(hatcherInRoom.Item.PrimaryKey == item.PrimaryKey){
                return $"{room.Name} already has a hatcher for {item.Name}.";
            }
        }
        
        var newHatcher = new ItemHatcher()
        {
            Item = item,
            MinQuantity = 1,
            MaxQuantity = 1,
            IntervalInTicks = 1
        };
        newHatcher.Inventories.Add(room.Inventory);

        await _itemHatcherRepository.CreateItemHatcher(newHatcher);

        return MessageStandard.Created($"item hatcher ({newHatcher.GetDetails()})")
            + $" {room.Name} is subscribed to this.";
    }
}
