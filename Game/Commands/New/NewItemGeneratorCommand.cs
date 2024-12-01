using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.New;

public class NewItemGeneratorCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasPickedBeing,
    ];

    private readonly IItemGeneratorRepository _itemGeneratorRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IPlayerState _state;

    private string ItemName => GetParameter(1);

    public NewItemGeneratorCommand(
        IItemGeneratorRepository itemGeneratorRepository,
        IItemRepository itemRepository,
        IPlayerState state
    )
    : base(regex: @"^new (.+) generator$")
    {
        _itemGeneratorRepository = itemGeneratorRepository;
        _itemRepository = itemRepository;
        _state = state;
    }

    public override async Task<string> Invoke()
    {
        var item = await _itemRepository.FindItem(ItemName);
        if (item is null)
        {
            return $"{ItemName} does not exist.";
        }

        var generator = new ItemGenerator()
        {
            Item = item,
            MinQuantity = 1,
            MaxQuantity = 1,
            IntervalInTicks = 1
        };
        var currentRoom = await _state.GetRoom();
        generator.Inventories.Add(currentRoom.Inventory);

        await _itemGeneratorRepository.CreateItemGenerator(generator);

        return MessageStandard.Created("item generator", ItemName);
    }
}
