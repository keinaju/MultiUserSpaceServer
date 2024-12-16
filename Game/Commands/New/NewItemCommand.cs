using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.New;

public class NewItemCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder
    ];

    protected override string Description => "Creates a new item.";

    private readonly IGameResponse _response;
    private readonly IItemRepository _itemRepository;

    public NewItemCommand(
        IGameResponse response,
        IItemRepository itemRepository
    )
    : base(regex: @"^new item$")
    {
        _response = response;
        _itemRepository = itemRepository;
    }

    public override async Task Invoke()
    {
        var newItem = new Item() { Name = string.Empty };

        var itemInDb = await _itemRepository.CreateItem(newItem);
        itemInDb.Name = $"i{itemInDb.PrimaryKey}";
        await _itemRepository.UpdateItem(itemInDb);

        _response.AddText(
            MessageStandard.Created(
                "item", itemInDb.Name
            )
        );
    }
}
