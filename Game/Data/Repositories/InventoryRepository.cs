using Microsoft.EntityFrameworkCore;
using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly GameContext _context;

    public InventoryRepository(GameContext context)
    {
        _context = context;
    }

    public async Task<Inventory> FindInventory(int primaryKey)
    {
        return await _context.Inventories
            .SingleAsync(inventory => inventory.PrimaryKey == primaryKey);
    }

    public async Task UpdateInventory(Inventory updatedInventory)
    {
        var inventoryInDb = await FindInventory(updatedInventory.PrimaryKey);
        inventoryInDb = updatedInventory;
        await _context.SaveChangesAsync();
    }
}
