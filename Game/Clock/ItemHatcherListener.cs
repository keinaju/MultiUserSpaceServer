using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Clock;

public class ItemHatcherListener : IGameClockListener
{
    private IInventoryRepository _inventoryRepository;
    private IItemHatcherRepository _itemHatcherRepository;

    public ItemHatcherListener(
        IInventoryRepository inventoryRepository,
        IItemHatcherRepository itemHatcherRepository
    )
    {
        _inventoryRepository = inventoryRepository;
        _itemHatcherRepository = itemHatcherRepository;
    }

    public Task GetTask(object sender, TickEventArgs eventArgs)
    {
        return UseItemHatchers(eventArgs);
    }

    /// <summary>
    /// Iterate all item hatchers in game and invoke item generations.
    /// </summary>
    /// <param name="eventArgs">Event arguments provided by game clock.</param>
    /// <returns>Task</returns>
    private async Task UseItemHatchers(TickEventArgs eventArgs)
    {
        var hatchers = await _itemHatcherRepository.FindAllItemHatchers();

        foreach (var hatcher in hatchers)
        {
            // Generate items only on tick intervals defined in hatcher
            if(eventArgs.TickCount % (ulong)hatcher.IntervalInTicks == 0)
            {
                await GenerateItem(hatcher);
            }
        }
    }

    /// <summary>
    /// Method that is called on each item hatcher on interval defined 
    /// by hatcher. Chooses randomly one inventory from all subscribed 
    /// inventories and adds specified item and quantity.
    /// </summary>
    /// <param name="hatcher">Item hatcher to invoke.</param>
    /// <returns>Task</returns>
    private async Task GenerateItem(ItemHatcher hatcher)
    {
        if (hatcher.Inventories.Count == 0) return;

        // Choose one random inventory from hatcher
        var randomIndex = new Random().Next(0, hatcher.Inventories.Count);
        var randomInventory = hatcher.Inventories.ElementAt(randomIndex);

        // Randomize amount of items
        var randomQuantity = new Random().Next(
            hatcher.MinimumQuantity,
            hatcher.MaximumQuantity + 1
        );

        // Populate item stacks
        randomInventory = await _inventoryRepository
            .FindInventory(randomInventory.PrimaryKey);

        // Do not generate, if inventory already contains this item
        if (randomInventory.Contains(hatcher.Item, 1)) return;

        randomInventory.AddItems(hatcher.Item, randomQuantity);
        await _inventoryRepository.UpdateInventory(randomInventory);
    }
}
