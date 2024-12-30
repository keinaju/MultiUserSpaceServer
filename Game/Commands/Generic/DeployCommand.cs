using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Generic;

public class DeployCommand : IGameCommand
{
    public string HelpText => "Deploys an item to a being.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserHasSelectedBeing
    ];

    public Regex Regex => new("^deploy (.+)$");

    private string ItemNameInInput =>
    _userInput.GetGroup(this.Regex, 1);

    private Being CurrentBeing => _player.GetSelectedBeing();

    private Inventory CurrentInventory => CurrentBeing.Inventory;

    private Room CurrentRoom => _player.GetCurrentRoom();

    private readonly IBeingRepository _beingRepo;
    private readonly IDeploymentRepository _deployRepo;
    private readonly IInventoryRepository _inventoryRepo;
    private readonly IItemRepository _itemRepo;
    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IRoomRepository _roomRepo;
    private readonly IUserInput _userInput;

    public DeployCommand(
        IBeingRepository beingRepo,
        IDeploymentRepository deployRepo,
        IInventoryRepository inventoryRepo,
        IItemRepository itemRepo,
        IPlayerState player,
        IResponsePayload response,
        IRoomRepository roomRepo,
        IUserInput userInput
    )
    {
        _beingRepo = beingRepo;
        _deployRepo = deployRepo;
        _inventoryRepo = inventoryRepo;
        _itemRepo = itemRepo;
        _player = player;
        _response = response;
        _roomRepo = roomRepo;
        _userInput = userInput;
    }

    public async Task Run()
    {
        var item = await _itemRepo.FindItem(ItemNameInInput);
        if(item is null)
        {
            _response.AddText(
                Message.DoesNotExist("item", ItemNameInInput)
            );
            return;
        }

        var deploy = await _deployRepo.FindDeploymentByItem(item);
        if(deploy is null)
        {
            _response.AddText(
                $"{item.Name} is not deployable."
            );
            return;
        }

        if(!CurrentInventory.Contains(item, 1))
        {
            _response.AddText(
                Message.DoesNotHave(
                    CurrentBeing.Name,
                    Message.Quantity(item.Name, 1)
                )
            );
            return;
        }

        await RemoveItem(item);
        var clone = await CreateClone(deploy);

        _response.AddText(
            $"{CurrentBeing.Name} deployed {item.Name} to {clone.Name}."
        );
    }

    private async Task RemoveItem(Item item)
    {
        CurrentInventory.RemoveItems(item, 1);

        await _inventoryRepo.UpdateInventory(CurrentInventory);
    }

    private async Task<Being> CreateClone(Deployment deployment)
    {
        var clone = deployment.Prototype.Clone();
        clone.Name = await _beingRepo.GetUniqueBeingName(clone.Name);
        clone.CreatedByUser = CurrentBeing.CreatedByUser;
        clone.InRoom = CurrentRoom;
        clone = await _beingRepo.CreateBeing(clone);

        await RenameInsideRoom(clone);

        return clone;
    }

    private async Task RenameInsideRoom(Being being)
    {
        var insideRoom = being.RoomInside;

        if(insideRoom is not null)
        {
            insideRoom.Name = await _roomRepo
            .GetUniqueRoomName(insideRoom.Name);

            await _roomRepo.UpdateRoom(insideRoom);
        }
    }
}
