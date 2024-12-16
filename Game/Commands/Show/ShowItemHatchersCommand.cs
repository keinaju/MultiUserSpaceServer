using System;
using MUS.Game.Data;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowItemHatchersCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasSelectedBeing
    ];

    protected override string Description =>
        "Shows all item hatchers the current room has subscribed to.";

    private readonly IGameResponse _response;
    private readonly IItemHatcherRepository _itemHatcherRepository;
    private readonly IPlayerState _state;

    public ShowItemHatchersCommand(
        IGameResponse response,
        IItemHatcherRepository itemHatcherRepository,
        IPlayerState state
    )
    : base(regex: @"^show hatchers in room$")
    {
        _itemHatcherRepository = itemHatcherRepository;
        _response = response;
        _state = state;
    }

    public override async Task Invoke()
    {
        var room = await _state.GetRoom();
        var hatchers = await _itemHatcherRepository
        .FindItemHatchersByInventory(room.Inventory);

        var details = new List<string>();
        foreach(var hatcher in hatchers)
        {
            details.Add(hatcher.GetDetails());
        }

        _response.AddText(
            $"{room.Name} is subscribed to hatchers: "
            + $"{MessageStandard.List(details)}."
        );
    }
}
