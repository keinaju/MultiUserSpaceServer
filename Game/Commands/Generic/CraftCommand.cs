using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Generic;

public class CraftCommand : IGameCommand
{
    public string HelpText => "Crafts an item from components.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserHasSelectedBeing
    ];

    public Regex Regex => new("^craft (.+)$");

    private string ItemNameInInput =>
    _userInput.GetGroup(this.Regex, 1);

    private Being CurrentBeing =>
    _player.GetSelectedBeing();

    private Inventory CurrentInventory =>
    CurrentBeing.Inventory;

    private readonly IInventoryRepository _inventoryRepo;
    private readonly IItemRepository _itemRepo;
    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IUserInput _userInput;
    private Item? _validItem = null;

    public CraftCommand(
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
        if(await IsValid())
        {
            await CraftItem();
        }
    }

    private async Task<bool> IsValid()
    {
        var item = await _itemRepo.FindItem(ItemNameInInput);
        if(item is null)
        {
            _response.AddText(
                Message.DoesNotExist("item", ItemNameInInput)
            );
            return false;
        }

        if(item.CraftPlan is null)
        {
            _response.AddText(
                $"{item.Name} can not be crafted."
            );
            return false;
        }

        if(item.CraftPlan.Components.Count == 0)
        {
            _response.AddText(
                Message.DoesNotHave(
                    $"{item.Name}'s craft plan", "components"
                )
            );
            return false;
        }

        foreach(var component in item.CraftPlan.Components)
        {
            if(!CurrentInventory.Contains(component.Item, component.Quantity))
            {
                _response.AddText(
                    Message.DoesNotHave(
                        $"{CurrentBeing.Name}'s inventory",
                        Message.Quantity(
                            component.Item.Name, component.Quantity
                        )
                    )
                );
                return false;
            }
        }

        _validItem = item;

        return true;
    }

    private async Task CraftItem()
    {
        var craftPlan = _validItem!.CraftPlan;

        // Remove components from inventory
        foreach(var component in craftPlan!.Components)
        {
            CurrentInventory.RemoveItems(
                component.Item, component.Quantity
            );
        }

        // Add product in inventory
        CurrentInventory.AddItems(craftPlan.Product, 1);

        await _inventoryRepo
        .UpdateInventory(CurrentInventory);

        _response.AddText(
            $"{CurrentBeing.Name} crafted {craftPlan.MadeOf()} to "
            + $"{Message.Quantity(craftPlan.Product.Name, 1)}."
        );
    }
}
