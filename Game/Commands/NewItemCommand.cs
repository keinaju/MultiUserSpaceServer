using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Commands;

public class NewItemCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn
    ];

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
        itemInDb.Name = $"I-{itemInDb.PrimaryKey}";
        await _itemRepository.UpdateItem(itemInDb);

        return $"{itemInDb.Name} was created.";
    }
}
