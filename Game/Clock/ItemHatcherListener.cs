using Microsoft.EntityFrameworkCore;
using MUS.Game.Data;
using MUS.Game.Data.Models;

namespace MUS.Game.Clock;

public class ItemHatcherListener : IGameClockListener
{
    private readonly GameContext _context;

    public ItemHatcherListener(
        GameContext context
    )
    {
        _context = context;
    }

    public Task GetTask(object sender, TickEventArgs eventArgs)
    {
        return UseItemHatchers(eventArgs);
    }

    /// <summary>
    /// Iterate all item hatchers in the game and invoke item generations.
    /// </summary>
    /// <param name="eventArgs">Event arguments provided by game clock.</param>
    /// <returns>Task</returns>
    private async Task UseItemHatchers(TickEventArgs eventArgs)
    {
        var hatchers = await _context.ItemHatchers.ToListAsync();

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
    /// inventories and adds a specified item and quantity.
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

        // Do not generate if inventory already contains this item
        if (randomInventory.Contains(hatcher.Item, 1)) return;

        await randomInventory.AddItems(hatcher.Item, randomQuantity);
    }
}
