using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Is;

public class ItemHatcherQuantityIsCommand : IGameCommand
{
    public string HelpText => "Sets the quantities for an item hatcher.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder,
        Condition.UserHasSelectedBeing
    ];

    public Regex Regex => new(
        @"^hatcher (.+) quantity is (\d+) to (\d+)$"
    );

    private string ItemNameInInput =>
    _userInput.GetGroup(this.Regex, 1);

    private string MinimumQuantityInInput =>
    _userInput.GetGroup(this.Regex, 2);

    private string MaximumQuantityInInput =>
    _userInput.GetGroup(this.Regex, 3);

    private Room CurrentRoom => _player.GetCurrentRoom();

    private readonly IItemHatcherRepository _itemHatcherRepo;
    private readonly IItemRepository _itemRepo;
    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IUserInput _userInput;

    public ItemHatcherQuantityIsCommand(
        IItemHatcherRepository itemHatcherRepo,
        IItemRepository itemRepo,
        IPlayerState player,
        IResponsePayload response,
        IUserInput userInput
    )
    {
        _itemHatcherRepo = itemHatcherRepo;
        _itemRepo = itemRepo;
        _player = player;
        _response = response;
        _userInput = userInput;
    }

    public async Task Run()
    {
        var minimumQuantity = GetParsedQuantity(MinimumQuantityInInput);
        if(minimumQuantity is null)
        {
            _response.AddText(
                Message.Invalid(MinimumQuantityInInput, "quantity")
            );
            return;
        }

        var maximumQuantity = GetParsedQuantity(MaximumQuantityInInput);
        if(maximumQuantity is null)
        {
            _response.AddText(
                Message.Invalid(MaximumQuantityInInput, "quantity")
            );
            return;
        }

        if(minimumQuantity > maximumQuantity)
        {
            _response.AddText(
                $"Minimum quantity can not be bigger than maximum quantity."
            );
            return;
        }

        var item = await _itemRepo.FindItem(ItemNameInInput);
        if(item is null)
        {
            _response.AddText(
                Message.DoesNotExist("item", ItemNameInInput)
            );
            return;
        }

        var itemHatcher = CurrentRoom.Inventory.GetItemHatcher(item);
        if(itemHatcher is null)
        {
            _response.AddText(
                $"{CurrentRoom.Name} is not subscribed to {item.Name} hatcher."
            );
            return;
        }

        await SetItemHatcherQuantities(
            itemHatcher,
            (int)minimumQuantity,
            (int)maximumQuantity
        );

        _response.AddText(
            Message.Set("item hatcher", itemHatcher.Show())
        );
    }

    private int? GetParsedQuantity(string input)
    {
        bool success = int.TryParse(
            input, out int result
        );

        if(success && result > 0)
        {
            return result;
        }
        else
        {
            return null;
        }
    }

    private async Task SetItemHatcherQuantities(
        ItemHatcher itemHatcher,
        int minimumQuantity,
        int maximumQuantity
    )
    {
        itemHatcher.MinimumQuantity = minimumQuantity;
        itemHatcher.MaximumQuantity = maximumQuantity;

        await _itemHatcherRepo.UpdateItemHatcher(itemHatcher);
    }
}
