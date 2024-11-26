using Microsoft.EntityFrameworkCore;
using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public class ItemStackRepository : IItemStackRepository
{
    private readonly GameContext _context;

    public ItemStackRepository(GameContext context)
    {
        _context = context;
    }

    public async Task<ItemStack?> FindItemStack(int inventoryPrimaryKey, int itemPrimaryKey)
    {
        try
        {
            return await _context.ItemsStacks
                .SingleAsync(stack =>
                    stack.InventoryPrimaryKey == inventoryPrimaryKey
                    && stack.ItemPrimaryKey == itemPrimaryKey
                );
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    public async Task SetItemStack(ItemStack itemStack)
    {
        var itemStackInDb = await FindItemStack(itemStack.Inventory.PrimaryKey, itemStack.Item.PrimaryKey);
        if (itemStackInDb is null)
        {
            await _context.ItemsStacks.AddAsync(itemStack);
        }
        else
        {
            itemStackInDb.Quantity += itemStack.Quantity;
        }
        await _context.SaveChangesAsync();
    }
}
