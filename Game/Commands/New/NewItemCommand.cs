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

    protected override string Description =>
        "Creates a new item.";

    private readonly IItemRepository _itemRepository;

    public NewItemCommand(
        IItemRepository itemRepository
    )
    : base(regex: @"^new item$")
    {
        _itemRepository = itemRepository;
    }

    public override async Task<string> Invoke()
    {
        Item newItem = new Item() { Name = string.Empty };

        var itemInDb = await _itemRepository.CreateItem(newItem);
        itemInDb.Name = $"i{itemInDb.PrimaryKey}";
        await _itemRepository.UpdateItem(itemInDb);

        return MessageStandard.Created("item", itemInDb.Name);
    }
}
