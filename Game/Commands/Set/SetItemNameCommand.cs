using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Set;

public class SetItemNameCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
    ];

    private readonly IItemRepository _itemRepository;
    private string OldItemName => GetParameter(1);
    private string NewItemName => GetParameter(2);

    public SetItemNameCommand(IItemRepository itemRepository)
    : base(regex: @"^set item (.+) name (.+)$")
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