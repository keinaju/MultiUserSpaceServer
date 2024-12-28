using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Is;

public class ItemDescriptionIsCommand : IGameCommand
{
    public string HelpText =>
    "Sets the description of an item.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder
    ];

    public Regex Regex => new("^item (.+) description is (.+)$");

    private string ItemNameInInput =>
    _userInput.GetGroup(this.Regex, 1);
    
    private string DescriptionInInput =>
    _userInput.GetGroup(this.Regex, 2);

    private readonly IItemRepository _itemRepo;
    private readonly IResponsePayload _response;
    private readonly IUserInput _userInput;

    public ItemDescriptionIsCommand(
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
        var item = await _itemRepo.FindItem(ItemNameInInput);
        if(item is null)
        {
            _response.AddText(
                Message.DoesNotExist("item", ItemNameInInput)
            );
            return;
        }

        await SetItemDescription(item);

        _response.AddText(
            Message.Set(
                $"{item.Name}'s description",
                DescriptionInInput
            )
        );
    }

    private async Task SetItemDescription(Item item)
    {
        item.Description = DescriptionInInput;

        await _itemRepo.UpdateItem(item);
    }
}