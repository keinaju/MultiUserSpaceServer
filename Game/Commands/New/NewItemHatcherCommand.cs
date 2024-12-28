using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.New;

public class NewItemHatcherCommand : IGameCommand
{
    public string HelpText => "Creates a new item hatcher.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder
    ];

    public Regex Regex => new("^new hatcher (.+)$");

    private string ItemNameInInput =>
    _userInput.GetGroup(this.Regex, 1);

    private Room CurrentRoom => _player.GetCurrentRoom();

    private Inventory RoomInventory => CurrentRoom.Inventory;

    private readonly IItemHatcherRepository _itemHatcherRepo;
    private readonly IItemRepository _itemRepo;
    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IUserInput _userInput;

    public NewItemHatcherCommand(
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
        if(await IsValid())
        {
            var itemHatcher = await CreateItemHatcher();

            await AddRoomInventoryInHatcher(itemHatcher);

            _response.AddText(
                Message.Created(
                    $"{itemHatcher.Item.Name} item hatcher"
                )
            );
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

        if(RoomInventory.GetItemHatcher(item) is not null)
        {
            _response.AddText(
                $"{CurrentRoom.Name} already has a hatcher for {item.Name}."
            );

            return false;
        }

        return true;
    }

    private async Task<ItemHatcher> CreateItemHatcher()
    {
        var item = await _itemRepo.FindItem(ItemNameInInput);

        return await _itemHatcherRepo.CreateItemHatcher(
            new ItemHatcher()
            {
                Item = item!,
                MinimumQuantity = 1,
                MaximumQuantity = 1,
                IntervalInTicks = 1
            }
        );
    }

    private async Task AddRoomInventoryInHatcher(ItemHatcher itemHatcher)
    {
        itemHatcher.Inventories.Add(RoomInventory);

        await _itemHatcherRepo.UpdateItemHatcher(itemHatcher);
    }
}
