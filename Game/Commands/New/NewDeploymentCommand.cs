using System;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.New;

public class NewDeploymentCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasPickedBeing,
        Prerequisite.UserIsBuilder
    ];

    private readonly IDeploymentRepository _deploymentRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IPlayerState _state;

    private string ItemName => GetParameter(1);

    protected override string Description =>
        "Creates a new deployment to convert an item into the current being.";

    public NewDeploymentCommand(
        IDeploymentRepository deploymentRepository,
        IItemRepository itemRepository,
        IPlayerState state
    )
    : base(regex: @"^new deploy (.+)$")
    {
        _deploymentRepository = deploymentRepository;
        _itemRepository = itemRepository;
        _state = state;
    }

    public override async Task<string> Invoke()
    {
        var item = await _itemRepository.FindItem(ItemName);
        if(item is null)
        {
            return MessageStandard.DoesNotExist("Item", ItemName);
        }

        var deploymentInDb = await _deploymentRepository
            .FindDeploymentByItem(item);
        if(deploymentInDb is not null)
        {
            return $"{item.Name} already has a deployment to "
            + $"{deploymentInDb.Prototype.Name}.";
        }

        var being = await _state.GetBeing();

        var deploy = await _deploymentRepository.CreateDeployment(
            new Deployment() { Item = item, Prototype = being }
        );

        return MessageStandard.Created(
            $"deployment from {item.Name} to {being.Name}"
        );
    }
}
