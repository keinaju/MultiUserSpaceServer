using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowItemCommand : IGameCommand
{
    public string HelpText => "Shows details about an item.";

    public Condition[] Conditions => [];

    public Regex Regex => new("^(show|s) item (.+)$");

    private string ItemName => _userInput.GetGroup(this.Regex, 2);

    private readonly IItemRepository _itemRepo;
    private readonly IResponsePayload _response;
    private readonly IUserInput _userInput;

    public ShowItemCommand(
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
        var item = await _itemRepo.FindItem(ItemName);

        if(item is not null)
        {
            _response.AddList(item.Show());
        }
        else
        {
            _response.AddText(
                Message.DoesNotExist("item", ItemName)
            );
        }
    }
}
