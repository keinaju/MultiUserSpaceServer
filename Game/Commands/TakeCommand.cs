using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands;

public class TakeCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasPickedBeing,
    ];

    private readonly IInventoryRepository _inventoryRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IPlayerState _state;

    private string QuantityInUserInput => GetParameter(1);
    private string ItemNameInUserInput => GetParameter(2);

    protected override string Description =>
        "Takes items from the current room's inventory.";

    private Item? _item = null;
    private int? _quantity = null;

    public TakeCommand(
        IInventoryRepository inventoryRepository,
        IItemRepository itemRepository,
        IPlayerState state
    )
    : base(regex: @"^take (\d+) (.+)$")
    {
        _inventoryRepository = inventoryRepository;
        _itemRepository = itemRepository;
        _state = state;
    }

    public override async Task<string> Invoke()
    {
        var errorMessage = await Validate();
        if (errorMessage is not null) return errorMessage;

        var currentRoom = await _state.GetRoom();
        // Populate room inventory
        var roomInventory = await _inventoryRepository.FindInventory(
            currentRoom.Inventory.PrimaryKey
        );

        if(roomInventory.Contains(_item!, (int)_quantity!))
        {
            var being = await _state.GetBeing();
            await GiveItemsFromRoom(being, roomInventory);
            return $"{being.Name} took {MessageStandard.Quantity(_item!.Name, (int)_quantity!)}.";
        }

        return MessageStandard.DoesNotContain(
            currentRoom.Name,
            MessageStandard.Quantity(ItemNameInUserInput, (int)_quantity)
        );
    }

    private async Task<string?> Validate()
    {
        _quantity = GetParsedQuantity();
        if(_quantity is null)
        {
            return MessageStandard
                .Invalid(QuantityInUserInput, "quantity");
        }

        _item = await _itemRepository.FindItem(ItemNameInUserInput);
        if(_item is null)
        {
            return MessageStandard.Invalid(ItemNameInUserInput, "item");
        }

        return null;
    }

    private int? GetParsedQuantity()
    {
        if (int.TryParse(QuantityInUserInput, out int quantity))
        {
            if (quantity > 0) return quantity;
        }

        return null;
    }

    private async Task GiveItemsFromRoom(Being being, Inventory roomInventory)
    {
        // Populate player inventory
        var beingInventory = await _inventoryRepository.FindInventory(
            being.Inventory.PrimaryKey
        );

        roomInventory.TransferTo(beingInventory, _item!, (int)_quantity!);
        await _inventoryRepository.UpdateInventory(roomInventory);
        await _inventoryRepository.UpdateInventory(beingInventory);
    }
}
