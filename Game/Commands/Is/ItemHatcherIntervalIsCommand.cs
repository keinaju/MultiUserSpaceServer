using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Session;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Is;

public class ItemHatcherIntervalIsCommand : IGameCommand
{
    public string HelpText => "Sets the interval for an item hatcher.";

    public Condition[] Conditions =>
    [
        // Condition.UserIsSignedIn,
        // Condition.UserIsBuilder,
        // Condition.UserHasSelectedBeing
    ];

    public Regex Regex => new(@"^hatcher (.+) interval is (\d+)$");

    private string ItemNameInInput =>
    _input.GetGroup(this.Regex, 1);

    private string IntervalInInput =>
    _input.GetGroup(this.Regex, 2);

    private Room CurrentRoom => _player.GetCurrentRoom();

    private readonly IItemRepository _itemRepo;
    private readonly IItemHatcherRepository _itemHatcherRepo;
    private readonly IPlayerState _player;
    private readonly GameContext _context;
    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public ItemHatcherIntervalIsCommand(
        IItemRepository itemRepo,
        IItemHatcherRepository itemHatcherRepo,
        IPlayerState player,
        GameContext context,
        IResponsePayload response,
        IInputCommand input,
        ISessionService session
    )
    {
        _itemRepo = itemRepo;
        _itemHatcherRepo = itemHatcherRepo;
        _player = player;
        _context = context;
        _response = response;
        _input = input;
        _session = session;
    }

    public async Task Run()
    {
        _response.AddResult(
            await ItemHatcherIntervalIs()
        );
        // bool success = int.TryParse(
        //     IntervalInInput, out int interval
        // );
        // if(!success || interval <= 0)
        // {
        //     _response.AddText(
        //         Message.Invalid(IntervalInInput, "interval")
        //     );
        //     return;
        // }

        // var item = await _itemRepo.FindItem(ItemNameInInput);
        // if(item is null)
        // {
        //     _response.AddText(
        //         Message.DoesNotExist("item", ItemNameInInput)
        //     );
        //     return;
        // }

        // var itemHatcher =
        // CurrentRoom.Inventory.GetItemHatcher(item);
        // if(itemHatcher is null)
        // {
        //     _response.AddText(
        //         $"{CurrentRoom.Name} is not subscribed to {item.Name} hatcher."
        //     );
        //     return;
        // }

        // await SetItemHatcherInterval(itemHatcher, interval);

        // _response.AddText(
        //     Message.Set("item hatcher", itemHatcher.Show())
        // );
    }

    private async Task<CommandResult> ItemHatcherIntervalIs()
    {
        if(_session.AuthenticatedUser is not null)
        {
            return await _session.AuthenticatedUser
            .ItemHatcherIntervalIs(ItemNameInInput, IntervalInInput);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }

    // private async Task SetItemHatcherInterval(
    //     ItemHatcher itemHatcher, int interval
    // )
    // {
    //     // itemHatcher.IntervalInTicks = interval;

    //     // await _itemHatcherRepo.UpdateItemHatcher(itemHatcher);
    // }
}
