using System;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Session;
using MUS.Game.Utilities;

namespace MUS.Game.Commands;

public class DeployCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasSelectedBeing
    ];

    private readonly IBeingRepository _beingRepository;
    private readonly IDeploymentRepository _deploymentRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IPlayerState _state;
    private readonly ISessionService _session;

    private string ItemName => GetParameter(1);

    protected override string Description =>
        "Converts an item into a being.";

    public DeployCommand(
        IBeingRepository beingRepository,
        IDeploymentRepository deploymentRepository,
        IInventoryRepository inventoryRepository,
        IItemRepository itemRepository,
        IPlayerState state,
        ISessionService session
    )
    : base(regex: @"^deploy (.+)$")
    {
        _beingRepository = beingRepository;
        _deploymentRepository = deploymentRepository;
        _inventoryRepository = inventoryRepository;
        _itemRepository = itemRepository;
        _state = state;
        _session = session;
    }

    public override async Task<string> Invoke()
    {
        var item = await _itemRepository.FindItem(ItemName);
        if(item is null)
        {
            return MessageStandard.DoesNotExist("Item", ItemName);
        }

        var deployment = await _deploymentRepository
            .FindDeploymentByItem(item);
        if(deployment is null)
        {
            return MessageStandard.DoesNotContain(ItemName, "a deployment");
        }

        var being = await _state.GetBeing();
        var inventory = await _state.GetInventory();
        if(!inventory.Contains(item, 1))
        {
            return MessageStandard.DoesNotContain(
                $"{being.Name}'s inventory",
                MessageStandard.Quantity(item.Name, 1)
            );
        }

        inventory.RemoveItems(item, 1);
        await _inventoryRepository.UpdateInventory(inventory);

        var clone = deployment.Prototype.Clone();
        clone.CreatedByUser = _session.AuthenticatedUser!;
        clone.InRoom = being.InRoom;
        clone = await _beingRepository.CreateBeing(clone);
        clone.Name = $"d{clone.PrimaryKey}";
        await _beingRepository.UpdateBeing(clone);

        return $"{being.Name} deployed {item.Name} to {clone.Name}.";
    }
}
