using System;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowCraftPlanCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn
    ];

    private readonly ICraftPlanRepository _craftPlanRepository;
    private readonly IItemRepository _itemRepository;

    private CraftPlan? _craftPlan = null;
    private Item? _product = null;

    private string ItemName => GetParameter(1);

    public ShowCraftPlanCommand(
        ICraftPlanRepository craftPlanRepository,
        IItemRepository itemRepository
    )
    : base(regex: @"^show (.+) craft plan$")
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

        return $"{_product!.Name} is made of {_craftPlan!.IsMadeOf()}.";
    }

    private async Task<string> Validate()
    {
        _product = await _itemRepository.FindItem(ItemName);
        if(_product is null)
        {
            return MessageStandard.DoesNotExist("Item", ItemName);
        }

        _craftPlan = await _craftPlanRepository
            .FindCraftPlanByProduct(_product);
        if(_craftPlan is null)
        {
            return $"{ItemName} is not an item that can be crafted.";
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
}
