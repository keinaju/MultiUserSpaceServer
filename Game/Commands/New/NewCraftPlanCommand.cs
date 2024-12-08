using System;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.New;

public class NewCraftPlanCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder
    ];

    private readonly ICraftPlanRepository _craftPlanRepository;
    private readonly IItemRepository _itemRepository;
    private string ItemName => GetParameter(1);

    protected override string Description =>
        "Creates a new plan for crafting an item.";

    public NewCraftPlanCommand(
        ICraftPlanRepository craftPlanRepository,
        IItemRepository itemRepository
    )
    : base(regex: @"^new (.+) craft plan$")
    {
        _craftPlanRepository = craftPlanRepository;
        _itemRepository = itemRepository;
    }

    public override async Task<string> Invoke()
    {
        var product = await _itemRepository.FindItem(ItemName);
        if(product is null)
        {
            return MessageStandard.DoesNotExist("Item", ItemName);
        }

        // Prevent duplicate craft plans for one item
        var cpInDb = await _craftPlanRepository
            .FindCraftPlanByProduct(product);
        if(cpInDb is not null)
        {
            return $"{product.Name} already has a craft plan.";
        }

        var craftPlan = await _craftPlanRepository.CreateCraftPlan(
            new CraftPlan() { Product = product! }
        );

        return MessageStandard.Created(
            $"{product!.Name} craft plan"
        );
    }
}
