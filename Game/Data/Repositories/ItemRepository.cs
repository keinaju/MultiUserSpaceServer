using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly GameContext _context;

    public ItemRepository(GameContext context)
    {
        _context = context;
    }

    public async Task<Item> CreateItem(Item item)
    {
        EntityEntry<Item> entry = await _context.Items.AddAsync(item);

        await _context.SaveChangesAsync();

        return entry.Entity;
    }

    public async Task<Item> FindItem(int primaryKey)
    {
        return await _context.Items.SingleAsync(
            item => item.PrimaryKey == primaryKey
        );
    }

    public async Task<Item?> FindItem(string itemName)
    {
        try
        {
            return await _context.Items.SingleAsync(
                item => item.Name == itemName
            );
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    public async Task<ICollection<Item>> FindItems()
    {
        return await _context.Items.ToListAsync();
    }

    public async Task UpdateItem(Item updatedItem)
    {
        var itemInDb = await FindItem(updatedItem.PrimaryKey);

        itemInDb = updatedItem;
        
        await _context.SaveChangesAsync();
    }
}
