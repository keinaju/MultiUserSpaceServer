using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Delete;

public class DeleteItemCommand : IGameCommand
{
    public string HelpText => "Deletes an item.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder
    ];

    public Regex Regex => new("^delete item (.+)$");

    private string ItemNameInInput => _userInput.GetGroup(this.Regex, 1);

    private readonly IItemRepository _itemRepo;
    private readonly IResponsePayload _response;
    private readonly IUserInput _userInput;

    public DeleteItemCommand(
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
        if(await IsValid())
        {
            await DeleteItem();
            Respond();
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

        return true;
    }

    private async Task DeleteItem()
    {
        var item = await _itemRepo.FindItem(ItemNameInInput);

        await _itemRepo.DeleteItem(item!.PrimaryKey);
    }

    private void Respond()
    {
        _response.AddText(
            Message.Deleted("item", ItemNameInInput)
        );
    }
}
