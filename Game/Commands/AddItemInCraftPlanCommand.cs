using System;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands;

public class AddItemInCraftPlanCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder,
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

    protected override string Description =>
        "Adds items in a craft plan.";

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
        var errorMessage = await Validate();
        if(errorMessage != string.Empty)
        {
            return errorMessage;
        }
        
        _craftPlan!.AddComponent(
            _componentItem!,
            _parsedComponentQuantity
        );

        await _craftPlanRepository.UpdateCraftPlan(_craftPlan);

        return GetResponse();
    }

    private async Task<string> Validate()
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
            + $"{CraftPlanItemName} is made of {_craftPlan!.IsMadeOf()}.";
    }
}
