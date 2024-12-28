using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.New;

public class NewDeploymentCommand : IGameCommand
{
    public string HelpText =>
    "Creates a new deployment from the current being to an item.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder,
        Condition.UserHasSelectedBeing
    ];

    private string ItemNameInInput =>
    _userInput.GetGroup(this.Regex, 1);

    public Regex Regex => new("^new deploy (.+)$");

    private Being CurrentBeing => _player.GetSelectedBeing();

    private readonly IDeploymentRepository _deployRepo;
    private readonly IItemRepository _itemRepo;
    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IUserInput _userInput;

    public NewDeploymentCommand(
        IDeploymentRepository deployRepo,
        IItemRepository itemRepo,
        IPlayerState player,
        IResponsePayload response,
        IUserInput userInput
    )
    {
        _deployRepo = deployRepo;
        _itemRepo = itemRepo;
        _player = player;
        _response = response;
        _userInput = userInput;
    }

    public async Task Run()
    {
        var item = await _itemRepo.FindItem(ItemNameInInput);
        if(item is null)
        {
            _response.AddText(
                Message.DoesNotExist("Item", ItemNameInInput)
            );
            return;
        }

        var deploy = await _deployRepo.FindDeploymentByItem(item);
        if(deploy is not null)
        {
            _response.AddText(
                $"{item.Name} already has a deployment to {deploy.Prototype.Name}."
            );
            return;
        }

        await CreateDeployment(item);

        _response.AddText(
            Message.Created(
                $"deployment from {item.Name} to {CurrentBeing.Name}"
            )
        );
    }

    private async Task CreateDeployment(Item item)
    {
        await _deployRepo.CreateDeployment(
            new Deployment()
            {
                Item = item,
                Prototype = CurrentBeing
            }
        );
    }
}
