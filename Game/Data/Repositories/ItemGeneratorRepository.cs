using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public class ItemGeneratorRepository : IItemGeneratorRepository
{
    private readonly GameContext _context;

    public ItemGeneratorRepository(GameContext context)
    {
        _context = context;
    }

    public async Task<ItemGenerator> CreateItemGenerator(ItemGenerator itemGenerator)
    {
        EntityEntry<ItemGenerator> entry = await _context.ItemGenerators.AddAsync(itemGenerator);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<List<ItemGenerator>> FindAllItemGenerators()
    {
        return await _context.ItemGenerators
            .Include(generator => generator.Inventories)
            .Include(generator => generator.Item)
            .ToListAsync();
    }
}
