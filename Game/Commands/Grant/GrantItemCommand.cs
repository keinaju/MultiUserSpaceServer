using System;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Grant;

public class GrantItemCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder,
        Prerequisite.UserHasSelectedBeing
    ];

    protected override string Description =>
    "Grants items to the current being.";

    private readonly IGameResponse _response;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IPlayerState _state;
    private string QuantityInUserInput => GetParameter(1);
    private string ItemName => GetParameter(2);

    public GrantItemCommand(
        IGameResponse response,
        IInventoryRepository inventoryRepository,
        IItemRepository itemRepository,
        IPlayerState state
    )
    : base(regex: @"^grant (\d+) item (.+)$")
    {
        _inventoryRepository = inventoryRepository;
        _itemRepository = itemRepository;
        _response = response;
        _state = state;
    }

    public override async Task Invoke()
    {
        var quantity = GetQuantity();
        if(quantity is null)
        {
            _response.AddText(
                MessageStandard.Invalid(
                    QuantityInUserInput, "quantity"
                )
            );
            return;
        }

        var item = await GetItem();
        if(item is null)
        {
            _response.AddText(
                MessageStandard.DoesNotExist(
                    "Item", ItemName
                )
            );
            return;
        }

        var inventory = await _state.GetInventory();
        inventory.AddItems(item, (int)quantity);
        await _inventoryRepository.UpdateInventory(inventory);

        var being = await _state.GetBeing();

        _response.AddText(
            $"{MessageStandard.Quantity(ItemName, (int)quantity)} was " +
            $"granted to {being.Name}'s inventory."
        );
    }

    private async Task<Item?> GetItem()
    {
        return await _itemRepository.FindItem(ItemName);
    }

    private int? GetQuantity()
    {
        if (int.TryParse(QuantityInUserInput, out int parsedQuantity))
        {
            if (parsedQuantity > 0) return parsedQuantity;
        }

        return null;
    }
}
