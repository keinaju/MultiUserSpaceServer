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
    private readonly IGameResponse _response;
    private readonly IItemRepository _itemRepository;
    private string ItemName => GetParameter(1);

    protected override string Description =>
    "Creates a new plan for crafting an item.";

    public NewCraftPlanCommand(
        ICraftPlanRepository craftPlanRepository,
        IGameResponse response,
        IItemRepository itemRepository
    )
    : base(regex: @"^new (.+) craft plan$")
    {
        _craftPlanRepository = craftPlanRepository;
        _itemRepository = itemRepository;
        _response = response;
    }

    public override async Task Invoke()
    {
        var product = await _itemRepository.FindItem(ItemName);
        if(product is null)
        {
            _response.AddText(
                MessageStandard.DoesNotExist("Item", ItemName)
            );
            return;
        }

        // Prevent duplicate craft plans for one item
        var cpInDb = await _craftPlanRepository
            .FindCraftPlanByProduct(product);
        if(cpInDb is not null)
        {
            _response.AddText(
                $"{product.Name} already has a craft plan."
            );
            return;
        }

        var craftPlan = await _craftPlanRepository.CreateCraftPlan(
            new CraftPlan() { Product = product! }
        );

        _response.AddText(
            MessageStandard.Created($"{product!.Name} craft plan")
        );
    }
}
