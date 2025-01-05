using System;
using System.Text.RegularExpressions;
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
    _input.GetGroup(this.Regex, 1);

    private string NewItemNameInInput =>
    _input.GetGroup(this.Regex, 2);

    private readonly IItemRepository _itemRepo;
    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;

    public ItemNameIsCommand(
        IItemRepository itemRepo,
        IResponsePayload response,
        IInputCommand input
    )
    {
        _itemRepo = itemRepo;
        _response = response;
        _input = input;
    }

    public async Task Run()
    {
        if(await IsValid())
        {
            await RenameItem();
            Respond();
        }
    }

    private async Task<bool> IsValid()
    {
        var item = await _itemRepo.FindItem(OldItemNameInInput);
        if(item is null)
        {
            _response.AddText(
                Message.DoesNotExist("item", OldItemNameInInput)
            );

            return false;
        }

        if(await _itemRepo.ItemNameIsReserved(NewItemNameInInput))
        {
            _response.AddText(
                Message.ReservedName("item name", NewItemNameInInput)
            );
            return false;
        }

        return true;
    }

    private async Task RenameItem()
    {
        var item = await _itemRepo.FindItem(OldItemNameInInput);

        item!.Name = NewItemNameInInput;

        await _itemRepo.UpdateItem(item);
    }

    private void Respond()
    {
        _response.AddText(
            Message.Renamed(
                OldItemNameInInput, NewItemNameInInput
            )
        );
    }
}
