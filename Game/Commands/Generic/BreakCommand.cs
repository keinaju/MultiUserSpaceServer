using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Generic;

public class BreakCommand : IGameCommand
{
    public string HelpText => "Breaks an item to components.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserHasSelectedBeing
    ];

    public Regex Regex => new("^break (.+)$");

    private string ItemNameInInput =>
    _input.GetGroup(this.Regex, 1);

    private Being CurrentBeing =>
    _player.GetSelectedBeing();

    private Inventory CurrentInventory =>
    CurrentBeing.Inventory;

    private readonly IInventoryRepository _inventoryRepo;
    private readonly IItemRepository _itemRepo;
    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;
    private Item? _validItem = null;

    public BreakCommand(
        IInventoryRepository inventoryRepo,
        IItemRepository itemRepo,
        IPlayerState player,
        IResponsePayload response,
        IInputCommand input)
    {
        _inventoryRepo = inventoryRepo;
        _itemRepo = itemRepo;
        _player = player;
        _response = response;
        _input = input;
    }

    public async Task Run()
    {
        if(await IsValid())
        {
            await BreakItem();
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
                $"{item.Name} can not be breaked."
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
        
        if(!CurrentInventory.Contains(item, 1))
        {
            _response.AddText(
                Message.DoesNotHave(
                    $"{CurrentBeing.Name}'s inventory",
                    Message.Quantity(
                        item.Name, 1
                    )
                )
            );
            return false;
        }

        _validItem = item;

        return true;
    }

    private async Task BreakItem()
    {
        var craftPlan = _validItem!.CraftPlan;

        // Remove product from inventory
        CurrentInventory.RemoveItems(_validItem, 1);

        // Add components in inventory
        foreach(var component in craftPlan!.Components)
        {
            CurrentInventory.AddItems(
                component.Item, component.Quantity
            );
        }

        await _inventoryRepo.UpdateInventory(CurrentInventory);

        _response.AddText(
            $"{CurrentBeing.Name} breaked {Message.Quantity(craftPlan.Product.Name, 1)} to {craftPlan.MadeOf()}."
        );
    }
}
