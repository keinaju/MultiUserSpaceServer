using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Set;

public class SetItemNameCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder
    ];

    private readonly IGameResponse _response;
    private readonly IItemRepository _itemRepository;
    private string OldItemName => GetParameter(1);
    private string NewItemName => GetParameter(2);

    protected override string Description =>
        "Sets a name for an item.";

    public SetItemNameCommand(
        IGameResponse response,
        IItemRepository itemRepository
    )
    : base(regex: @"^set item (.+) name (.+)$")
    {
        _itemRepository = itemRepository;
        _response = response;
    }

    public override async Task Invoke()
    {
        var item = await _itemRepository.FindItem(OldItemName);
        if(item is null)
        {
            _response.AddText(
                MessageStandard.DoesNotExist("Item", OldItemName)
            );
            return;
        }

        item.Name = NewItemName;
        await _itemRepository.UpdateItem(item);

        _response.AddText(
            MessageStandard.Renamed(OldItemName, NewItemName)
        );
    }
}
