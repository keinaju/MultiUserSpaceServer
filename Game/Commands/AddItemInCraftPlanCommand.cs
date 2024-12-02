using System;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands;

public class AddItemInCraftPlanCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasPickedBeing
    ];

    private readonly IItemRepository _itemRepository;
    private readonly ICraftPlanRepository _craftPlanRepository;
    private int _parsedComponentQuantity = 0;
    private Item? _componentItem = null;
    private CraftPlan? _craftPlan = null;

    private string QuantityInUserInput => GetParameter(1);
    private string ComponentItemName => GetParameter(2);
    private string CraftPlanItemName => GetParameter(3);

    public AddItemInCraftPlanCommand(
        ICraftPlanRepository craftPlanRepository,
        IItemRepository itemRepository
    )
    : base(regex: @"^add (\d+) (.+) in (.+) craft plan$")
    {
        _craftPlanRepository = craftPlanRepository;
        _itemRepository = itemRepository;
    }

    public override async Task<string> Invoke()
    {
        var errorMessage = await GetValidationResult();
        if(errorMessage != string.Empty)
        {
            return errorMessage;
        }

        // Strategy 1:
        // If craft plan already has this component,
        // add quantity to existing craft component.
        foreach(var componentInPlan in _craftPlan!.Components)
        {
            if(_componentItem!.PrimaryKey == componentInPlan.Item.PrimaryKey)
            {
                // The plan has already this component.
                // Add quantity to existing component:
                componentInPlan.Quantity += _parsedComponentQuantity;
                await _craftPlanRepository.UpdateCraftPlan(_craftPlan);
                return GetResponse();
            }
        }

        // Strategy 2:
        // If craft plan does not have this component,
        // create a new craft component.
        _craftPlan!.Components.Add(
            new CraftComponent()
            {
                Item = _componentItem!,
                Quantity = _parsedComponentQuantity
            }
        );
        await _craftPlanRepository.UpdateCraftPlan(_craftPlan);

        return GetResponse();
    }

    private async Task<string> GetValidationResult()
    {
        // Validate quantity in user input
        bool result = int.TryParse(QuantityInUserInput, out _parsedComponentQuantity);
        if(!result || _parsedComponentQuantity <= 0)
        {
            return MessageStandard.Invalid(QuantityInUserInput, "quantity");
        }

        // Validate that item exists (component)
        _componentItem = await _itemRepository.FindItem(ComponentItemName);
        if(_componentItem is null)
        {
            return MessageStandard.DoesNotExist("Item", ComponentItemName);
        }

        // Validate that item exists (product)
        var product = await _itemRepository.FindItem(CraftPlanItemName);
        if(product is null)
        {
            return MessageStandard.DoesNotExist("Item", CraftPlanItemName);
        }

        // Validate that component is not product
        if(_componentItem.PrimaryKey == product.PrimaryKey)
        {
            return "Component and product can not be the same item.";
        }

        // Validate that craft plan exists for product
        _craftPlan = await _craftPlanRepository.FindCraftPlanByProduct(product);
        if(_craftPlan is null)
        {
            return MessageStandard.DoesNotExist("Craft plan for", CraftPlanItemName);
        }

        return string.Empty;
    }

    private string GetResponse()
    {
        return 
            MessageStandard.Quantity(_componentItem!.Name, _parsedComponentQuantity)
            + $" was added to {CraftPlanItemName}'s craft plan. "
            + $"{_craftPlan!.IsMadeOf()}.";
    }
}
