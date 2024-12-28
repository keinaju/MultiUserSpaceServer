using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Generic;

public class TakeCommand : IGameCommand
{
    public string HelpText =>
    "Takes items from the current room's inventory.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserHasSelectedBeing
    ];

    public Regex Regex => new("^take (.+)$");

    private string ItemNameInInput =>
    _userInput.GetGroup(this.Regex, 1);

    private Being Being => _player.GetSelectedBeing();

    private Room Room => _player.GetCurrentRoom();

    private readonly IInventoryRepository _inventoryRepo;
    private readonly IItemRepository _itemRepo;
    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IUserInput _userInput;

    public TakeCommand(
        IInventoryRepository inventoryRepo,
        IItemRepository itemRepo,
        IPlayerState player,
        IResponsePayload response,
        IUserInput userInput
    )
    {
        _inventoryRepo = inventoryRepo;
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
                Message.DoesNotExist("item", ItemNameInInput)
            );

            return;
        }

        if(!Room.Inventory.Contains(item, 1))
        {
            _response.AddText(
                Message.DoesNotHave(Room.Name, ItemNameInInput)
            );

            return;
        }

        await TakeItem(item);

        _response.AddText(
            $"{Being.Name} took {Message.Quantity(item.Name, 1)}"
            + $" from {Room.Name}."
        );
    }

    private async Task TakeItem(Item item)
    {
        Room.Inventory.TransferTo(Being.Inventory, item, 1);

        await _inventoryRepo.UpdateInventory(Room.Inventory);
        await _inventoryRepo.UpdateInventory(Being.Inventory);
    }
}
