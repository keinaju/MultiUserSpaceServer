
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Clock;

public class ItemGeneratorListener : IGameClockListener
{
    private IItemGeneratorRepository _itemGeneratorRepository;
    private IItemStackRepository _itemStackRepository;

    public ItemGeneratorListener(
        IItemGeneratorRepository itemGeneratorRepository,
        IItemStackRepository itemStackRepository
    )
    {
        _itemGeneratorRepository = itemGeneratorRepository;
        _itemStackRepository = itemStackRepository;
    }

    public Task GetTask(object sender, TickEventArgs eventArgs)
    {
        return UseItemGenerators();
    }

    private async Task UseItemGenerators()
    {
        var generators = await _itemGeneratorRepository.GetAllItemGenerators();

        foreach (var generator in generators)
        {
            foreach (var inventory in generator.Inventories)
            {
                var newStack = new ItemStack()
                {
                    Item = generator.Item,
                    Inventory = inventory,
                    Quantity = 1
                };
                await _itemStackRepository.SetItemStack(newStack);
            }
        }
    }
}
