using System;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowItemCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    private readonly ICraftPlanRepository _craftPlanRepository;
    private readonly IDeploymentRepository _deploymentRepository;
    private readonly IItemRepository _itemRepository;

    private CraftPlan? _craftPlan = null;
    private Item? _product = null;

    private string ItemName => GetParameter(1);

    protected override string Description => "Shows an item and it's details.";

    public ShowItemCommand(
        ICraftPlanRepository craftPlanRepository,
        IDeploymentRepository deploymentRepository,
        IItemRepository itemRepository
    )
    : base(regex: @"^show item (.+)$")
    {
        _craftPlanRepository = craftPlanRepository;
        _deploymentRepository = deploymentRepository;
        _itemRepository = itemRepository;
    }

    public override async Task<string> Invoke()
    {
        var errorMessage = await Validate();
        if(errorMessage != string.Empty)
        {
            return errorMessage;
        }

        return _product!.ToString() + " " +
            await GetMadeOfText() + " " +
            await GetDeploymentText();
    }

    private async Task<string> Validate()
    {
        _product = await _itemRepository.FindItem(ItemName);
        if(_product is null)
        {
            return MessageStandard.DoesNotExist("Item", ItemName);
        }

        return string.Empty;
    }

    private async Task<string> GetMadeOfText()
    {
        _craftPlan = await _craftPlanRepository
            .FindCraftPlanByProduct(_product!);

        if(_craftPlan is null)
        {
            return $"{_product!.Name} is not an item that can be crafted.";
        }
        else if(_craftPlan.Components.Count == 0)
        {
            return MessageStandard.DoesNotContain(
                $"{_product!.Name}'s craft plan",
                "components"
            );
        }

        return $"{_product!.Name} is made of {_craftPlan!.IsMadeOf()}.";
    }

    private async Task<string> GetDeploymentText()
    {
        var deployment = await _deploymentRepository
            .FindDeploymentByItem(_product!);
        
        if(deployment is null)
        {
            return "";
        }

        return $"{deployment.Item.Name} can be deployed to {deployment.Prototype.Name}.";
    }
}
