using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Is;

public class ItemNameIsCommand : IGameCommand
{
    public string HelpText => "Renames an item.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder
    ];

    public Regex Regex => new("^item name (.+) is (.+)$");

    private string OldItemNameInInput =>
    _userInput.GetGroup(this.Regex, 1);

    private string NewItemNameInInput =>
    _userInput.GetGroup(this.Regex, 2);

    private readonly IItemRepository _itemRepo;
    private readonly IResponsePayload _response;
    private readonly IUserInput _userInput;

    public ItemNameIsCommand(
        IItemRepository itemRepo,
        IResponsePayload response,
        IUserInput userInput
    )
    {
        _itemRepo = itemRepo;
        _response = response;
        _userInput = userInput;
    }

    public async Task Run()
    {
        var item = await _itemRepo.FindItem(OldItemNameInInput);
        if(item is null)
        {
            _response.AddText(
                Message.DoesNotExist("item", OldItemNameInInput)
            );

            return;
        }

        await SetItemName(item);

        _response.AddText(
            Message.Renamed(
                OldItemNameInInput, NewItemNameInInput
            )
        );
    }

    private async Task SetItemName(Item item)
    {
        item.Name = NewItemNameInInput;

        await _itemRepo.UpdateItem(item);
    }
}
