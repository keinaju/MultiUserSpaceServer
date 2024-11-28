using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Rename;

public class RenameItemCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
    ];

    private readonly IItemRepository _itemRepository;
    private string OldItemName => GetParameter(1);
    private string NewItemName => GetParameter(2);

    public RenameItemCommand(IItemRepository itemRepository)
    : base(regex: @"^rename item (.+):(.+)$")
    {
        _itemRepository = itemRepository;
    }

    public override async Task<string> Invoke()
    {
        var item = await _itemRepository.FindItem(OldItemName);
        if(item is null)
        {
            return MessageStandard.DoesNotExist(OldItemName);
        }

        item.Name = NewItemName;
        await _itemRepository.UpdateItem(item);

        return MessageStandard.Renamed(OldItemName, NewItemName);
    }
}
