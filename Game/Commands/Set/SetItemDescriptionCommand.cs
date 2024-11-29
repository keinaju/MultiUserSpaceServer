using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Set;

public class SetItemDescriptionCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
    ];

    private readonly IItemRepository _itemRepository;
    private string ItemName => GetParameter(1);
    private string ItemDescription => GetParameter(2);

    public SetItemDescriptionCommand(IItemRepository itemRepository)
    : base(regex: @"^set item (.+) description (.+)$")
    {
        _itemRepository = itemRepository;
    }

    public override async Task<string> Invoke()
    {
        var item = await _itemRepository.FindItem(ItemName);
        if(item is null)
        {
            return MessageStandard.DoesNotExist(ItemName);
        }

        item.Description = ItemDescription;
        await _itemRepository.UpdateItem(item);

        return MessageStandard.Set($"{ItemName}'s description", ItemDescription);
    }
}