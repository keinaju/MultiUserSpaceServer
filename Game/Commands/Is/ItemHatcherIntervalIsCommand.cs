using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Is;

public class ItemHatcherIntervalIsCommand : IGameCommand
{
    public string HelpText => "Sets the interval for an item hatcher.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder,
        Condition.UserHasSelectedBeing
    ];

    public Regex Regex => new(@"^hatcher (.+) interval is (\d+)$");

    private string ItemNameInInput =>
    _userInput.GetGroup(this.Regex, 1);

    private string IntervalInInput =>
    _userInput.GetGroup(this.Regex, 2);

    private Room CurrentRoom => _player.GetCurrentRoom();

    private readonly IItemRepository _itemRepo;
    private readonly IItemHatcherRepository _itemHatcherRepo;
    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IUserInput _userInput;

    public ItemHatcherIntervalIsCommand(
        IItemRepository itemRepo,
        IItemHatcherRepository itemHatcherRepo,
        IPlayerState player,
        IResponsePayload response,
        IUserInput userInput
    )
    {
        _itemRepo = itemRepo;
        _itemHatcherRepo = itemHatcherRepo;
        _player = player;
        _response = response;
        _userInput = userInput;
    }

    public async Task Run()
    {
        bool success = int.TryParse(
            IntervalInInput, out int interval
        );
        if(!success || interval <= 0)
        {
            _response.AddText(
                Message.Invalid(IntervalInInput, "interval")
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

        var itemHatcher =
        CurrentRoom.Inventory.GetItemHatcher(item);
        if(itemHatcher is null)
        {
            _response.AddText(
                $"{CurrentRoom.Name} is not subscribed to {item.Name} hatcher."
            );
            return;
        }

        await SetItemHatcherInterval(itemHatcher, interval);

        _response.AddText(
            Message.Set("item hatcher", itemHatcher.Show())
        );
    }

    private async Task SetItemHatcherInterval(
        ItemHatcher itemHatcher, int interval
    )
    {
        itemHatcher.IntervalInTicks = interval;

        await _itemHatcherRepo.UpdateItemHatcher(itemHatcher);
    }
}
