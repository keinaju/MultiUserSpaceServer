using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Set;

public class SetItemDescriptionCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder
    ];

    private readonly IGameResponse _response;
    private readonly IItemRepository _itemRepository;
    private string ItemName => GetParameter(1);
    private string ItemDescription => GetParameter(2);

    protected override string Description =>
        "Sets a description for an item.";

    public SetItemDescriptionCommand(
        IGameResponse response,
        IItemRepository itemRepository
    )
    : base(regex: @"^set item (.+) description (.+)$")
    {
        _itemRepository = itemRepository;
        _response = response;
    }

    public override async Task Invoke()
    {
        var item = await _itemRepository.FindItem(ItemName);
        if(item is null)
        {
            _response.AddText(
                MessageStandard.DoesNotExist("Item", ItemName)
            );
            return;
        }

        item.Description = ItemDescription;
        await _itemRepository.UpdateItem(item);

        _response.AddText(
            MessageStandard.Set($"{ItemName}'s description", ItemDescription)
        );
    }
}
