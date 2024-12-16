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
        Prerequisite.UserHasSelectedBeing,
        Prerequisite.UserIsBuilder
    ];

    protected override string Description =>
    "Creates a new deployment to convert an item into the current being.";

    private string ItemName => GetParameter(1);

    private readonly IDeploymentRepository _deploymentRepository;
    private readonly IGameResponse _response;
    private readonly IItemRepository _itemRepository;
    private readonly IPlayerState _state;

    public NewDeploymentCommand(
        IDeploymentRepository deploymentRepository,
        IGameResponse response,
        IItemRepository itemRepository,
        IPlayerState state
    )
    : base(regex: @"^new deploy (.+)$")
    {
        _deploymentRepository = deploymentRepository;
        _itemRepository = itemRepository;
        _response = response;
        _state = state;
    }

    public override async Task Invoke()
    {
        var item = await _itemRepository.FindItem(ItemName);
        if(item is null)
        {
            _response.AddText(
                MessageStandard.DoesNotExist("Item", ItemName)
            );
            return;
        }

        var deploymentInDb = await _deploymentRepository
            .FindDeploymentByItem(item);
        if(deploymentInDb is not null)
        {
            _response.AddText(
                $"{item.Name} already has a deployment to "
                + $"{deploymentInDb.Prototype.Name}."
            );
            return; 
        }

        var being = await _state.GetBeing();

        var deploy = await _deploymentRepository.CreateDeployment(
            new Deployment() { Item = item, Prototype = being }
        );

        _response.AddText(
            MessageStandard.Created(
                $"deployment from {item.Name} to {being.Name}"
            )
        );
    }
}
