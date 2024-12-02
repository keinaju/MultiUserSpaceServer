using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands;

public class CraftCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasPickedBeing
    ];

    private readonly ICraftPlanRepository _craftPlanRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IPlayerState _state;

    private CraftPlan? _craftPlan = null;
    private Item? _product = null;

    private string ItemName => GetParameter(1);

    public CraftCommand(
        ICraftPlanRepository craftPlanRepository,
        IInventoryRepository inventoryRepository,
        IItemRepository itemRepository,
        IPlayerState state
    )
    :  base(regex: @"^craft (.+)$")
    {
        _craftPlanRepository = craftPlanRepository;
        _inventoryRepository = inventoryRepository;
        _itemRepository = itemRepository;
        _state = state;
    }

    public override async Task<string> Invoke()
    {
        var errorMessage = await Validate();
        if(errorMessage != string.Empty)
        {
            return errorMessage;
        }
        
        var being = await _state.GetBeing();
        var inventory = await _state.GetInventory();
        
        foreach(var component in _craftPlan!.Components)
        {
            if(!inventory.Contains(component.Item, component.Quantity))
            {
                return MessageStandard.DoesNotContain(
                    $"{being.Name}'s inventory",
                    MessageStandard.Quantity(
                        component.Item.Name, component.Quantity
                    )
                );
            }
        }

        // At this point inventory contains required items

        // Remove components from inventory
        foreach(var component in _craftPlan!.Components)
        {
            inventory.RemoveItems(component.Item, component.Quantity);
        }

        // Add product in inventory
        inventory.AddItems(_product!, 1);

        await _inventoryRepository.UpdateInventory(inventory);
        
        return $"{being.Name} crafted {_product!.Name} from {_craftPlan.IsMadeOf()}.";
    }

    private async Task<string> Validate()
    {
        _product = await _itemRepository.FindItem(ItemName);
        if(_product is null)
        {
            return MessageStandard.DoesNotExist("Item", ItemName);
        }

        _craftPlan = await _craftPlanRepository.FindCraftPlanByProduct(_product);
        if(_craftPlan is null)
        {
            return MessageStandard.DoesNotExist("Craft plan for", ItemName);
        }
        if(_craftPlan.Components.Count == 0)
        {
            return MessageStandard.DoesNotContain(
                $"{_product.Name}'s craft plan",
                "items"
            );
        }

        return string.Empty;
    }
}
