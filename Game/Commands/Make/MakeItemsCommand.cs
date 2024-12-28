using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Make;

public class MakeItemsCommand : IGameCommand
{
    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder,
        Condition.UserHasSelectedBeing
    ];

    public string HelpText =>
    "Creates a stack of items in the current being's inventory.";

    public Regex Regex => new(@"^make (\d+) (.+)$");

    private string ItemNameInInput =>
    _userInput.GetGroup(this.Regex, 2);

    private string QuantityInInput =>
    _userInput.GetGroup(this.Regex, 1);

    private string BeingName =>
    _player.GetSelectedBeing().Name;

    private Inventory Inventory =>
    _player.GetSelectedBeing().Inventory;

    private readonly IInventoryRepository _inventoryRepo;
    private readonly IItemRepository _itemRepo;
    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IUserInput _userInput;

    public MakeItemsCommand(
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
        var success = int.TryParse(
            QuantityInInput, out int quantity
        );
        if(!success || quantity <= 0)
        {
            _response.AddText(
                Message.Invalid(QuantityInInput, "quantity")
            );

            return;
        }

        var item = await _itemRepo.FindItem(ItemNameInInput);
        if (item is null)
        {
            _response.AddText(
                Message.DoesNotExist("item", ItemNameInInput)
            );

            return;
        }

        await MakeItems(item, quantity);

        _response.AddText(
            $"{BeingName} got {Message.Quantity(item.Name, quantity)}."
        );
    }

    private async Task MakeItems(Item item, int quantity)
    {
        Inventory.AddItems(item, quantity);

        await _inventoryRepo.UpdateInventory(Inventory);
    }
}
