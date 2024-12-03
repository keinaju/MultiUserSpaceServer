using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Clock;

public class ItemGeneratorListener : IGameClockListener
{
    private IInventoryRepository _inventoryRepository;
    private IItemGeneratorRepository _itemGeneratorRepository;

    public ItemGeneratorListener(
        IInventoryRepository inventoryRepository,
        IItemGeneratorRepository itemGeneratorRepository
    )
    {
        _inventoryRepository = inventoryRepository;
        _itemGeneratorRepository = itemGeneratorRepository;
    }

    public Task GetTask(object sender, TickEventArgs eventArgs)
    {
        return UseItemGenerators(eventArgs);
    }

    /// <summary>
    /// Iterate all item generators in game and invoke item generations.
    /// </summary>
    /// <param name="eventArgs">Event arguments provided by game clock.</param>
    /// <returns>Task</returns>
    private async Task UseItemGenerators(TickEventArgs eventArgs)
    {
        var generators = await _itemGeneratorRepository.FindAllItemGenerators();

        foreach (var generator in generators)
        {
            // Generate items only on tick intervals defined in generator
            if(eventArgs.TickCount % (ulong)generator.IntervalInTicks == 0)
            {
                await GenerateItem(generator);
            }
        }
    }

    /// <summary>
    /// Method that is called on each item generator on interval defined 
    /// by generator. Chooses randomly one inventory from all subscribed 
    /// inventories and adds specified item and quantity.
    /// </summary>
    /// <param name="generator">Item generator to invoke.</param>
    /// <returns>Task</returns>
    private async Task GenerateItem(ItemGenerator generator)
    {
        // Choose one random inventory from generator
        var randomIndex = new Random().Next(0, generator.Inventories.Count);
        var randomInventory = generator.Inventories.ElementAt(randomIndex);

        // Randomize amount of items
        var randomQuantity = new Random().Next(
            generator.MinQuantity,
            generator.MaxQuantity + 1
        );

        // Populate item stacks
        randomInventory = await _inventoryRepository
            .FindInventory(randomInventory.PrimaryKey);

        // Do not generate, if inventory already contains this item
        if (randomInventory.Contains(generator.Item, 1)) return;

        randomInventory.AddItems(generator.Item, randomQuantity);
        await _inventoryRepository.UpdateInventory(randomInventory);
    }
}
