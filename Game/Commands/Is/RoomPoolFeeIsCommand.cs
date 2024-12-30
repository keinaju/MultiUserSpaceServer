using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Is;

public class RoomPoolFeeIsCommand : IGameCommand
{
    public string HelpText => "Sets the fee item of a room pool.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder
    ];

    public Regex Regex => new("^pool (.+) fee is (.+)$");

    private string RoomPoolNameInInput =>
    _userInput.GetGroup(this.Regex, 1);

    private string ItemNameInInput =>
    _userInput.GetGroup(this.Regex, 2);

    private readonly IItemRepository _itemRepo;
    private readonly IResponsePayload _response;
    private readonly IRoomPoolRepository _roomPoolRepo;
    private readonly IUserInput _userInput;

    public RoomPoolFeeIsCommand(
        IItemRepository itemRepo,
        IResponsePayload response,
        IRoomPoolRepository roomPoolRepo,
        IUserInput userInput
    )
    {
        _itemRepo = itemRepo;
        _response = response;
        _roomPoolRepo = roomPoolRepo;
        _userInput = userInput;
    }

    public async Task Run()
    {
        if(await IsValid())
        {
            await SetRoomPoolFeeItem();
        }
    }

    private async Task<bool> IsValid()
    {
        if(await GetRoomPool() is null)
        {
            _response.AddText(
                Message.DoesNotExist("room pool", RoomPoolNameInInput)
            );
            return false;
        }

        if(await GetItem() is null)
        {
            _response.AddText(
                Message.DoesNotExist("item", ItemNameInInput)
            );
            return false;
        }

        return true;
    }

    private async Task SetRoomPoolFeeItem()
    {
        var roomPool = await GetRoomPool();

        roomPool!.FeeItem = await GetItem();

        await _roomPoolRepo.UpdateRoomPool(roomPool);

        _response.AddText(
            Message.Set(
                $"{roomPool.Name}'s fee item",
                roomPool.FeeItem!.Name
            )
        );
    }

    private async Task<RoomPool?> GetRoomPool()
    {
        return await _roomPoolRepo.FindRoomPool(RoomPoolNameInInput);
    }

    private async Task<Item?> GetItem()
    {
        return await _itemRepo.FindItem(ItemNameInInput);
    }
}
