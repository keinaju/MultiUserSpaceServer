using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands;

public class BreakOrCraftCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasSelectedBeing
    ];

    private readonly ICraftPlanRepository _craftPlanRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IPlayerState _state;

    private CraftPlan? _craftPlan = null;
    private Item? _product = null;
    private Being _being;
    private Inventory _inventory;

    private string BreakOrCraft => GetParameter(1);
    private string ItemName => GetParameter(2);

    protected override string Description =>
        "Breaks an item into components, or crafts an item from components.";

    public BreakOrCraftCommand(
        ICraftPlanRepository craftPlanRepository,
        IInventoryRepository inventoryRepository,
        IItemRepository itemRepository,
        IPlayerState state
    )
    :  base(regex: @"^(break|craft) (.+)$")
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
        
        _being = await _state.GetBeing();
        _inventory = await _state.GetInventory();

        if (BreakOrCraft == "break") return await BreakStrategy();
        else return await CraftStrategy();
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
                "components"
            );
        }

        return string.Empty;
    }

    private async Task<string> BreakStrategy()
    {
        if(!_inventory.Contains(_product!, 1))
        {
            return MessageStandard.DoesNotContain(
                $"{_being.Name}'s inventory",
                _product!.Name
            );
        }

        // At this point inventory contains required product

        // Remove product from inventory
        _inventory.RemoveItems(_product!, 1);

        // Add components in inventory
        foreach(var component in _craftPlan!.Components)
        {
            _inventory.AddItems(component.Item, component.Quantity);
        }

        await _inventoryRepository.UpdateInventory(_inventory);
        
        return $"{_being.Name} breaked {_product!.Name} to {_craftPlan.IsMadeOf()}.";
    }

    private async Task<string> CraftStrategy()
    {
        foreach(var component in _craftPlan!.Components)
        {
            if(!_inventory.Contains(component.Item, component.Quantity))
            {
                return MessageStandard.DoesNotContain(
                    $"{_being.Name}'s inventory",
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
            _inventory.RemoveItems(component.Item, component.Quantity);
        }

        // Add product in inventory
        _inventory.AddItems(_product!, 1);

        await _inventoryRepository.UpdateInventory(_inventory);
        
        return $"{_being.Name} crafted {_product!.Name} from {_craftPlan.IsMadeOf()}.";
    }
}
