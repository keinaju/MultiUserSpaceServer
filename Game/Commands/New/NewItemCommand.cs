using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.New;

public class NewItemCommand : IGameCommand
{
    public string HelpText => "Creates a new item.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder
    ];

    public Regex Regex => new("^new item$");
    
    private readonly IItemRepository _itemRepo;
    private readonly IResponsePayload _response;

    public NewItemCommand(
        IItemRepository itemRepo,
        IResponsePayload response
    )
    {
        _itemRepo = itemRepo;
        _response = response;
    }

    public async Task Run()
    {
        var item = await CreateItem();

        await SetItemName(item);

        _response.AddText(
            Message.Created("item", item.Name)
        );
    }

    private async Task<Item> CreateItem()
    {
        return await _itemRepo.CreateItem(
            new Item()
            {
                Name = string.Empty,
                Description = null,
                CraftPlan = null
            }
        );
    }

    private async Task SetItemName(Item item)
    {
        item.Name = $"i{item.PrimaryKey}";

        await _itemRepo.UpdateItem(item);
    }
}
