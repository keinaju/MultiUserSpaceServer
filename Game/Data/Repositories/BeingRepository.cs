using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public class BeingRepository : IBeingRepository
{
    private readonly GameContext _context;

    public BeingRepository(GameContext context)
    {
        _context = context;
    }

    public async Task<Being> CreateBeing(Being being)
    {
        EntityEntry<Being> entry = await _context.Beings.AddAsync(being);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<Being> FindBeing(int primaryKey)
    {
        return await _context.Beings
            .Include(being => being.Room)
            .Include(being => being.Inventory)
            .SingleAsync(being => being.PrimaryKey == primaryKey);
    }

    public async Task<List<Being>> FindBeingsByUser(User user)
    {
        return await _context.Beings
            .Where(e => e.CreatedByUser == user)
            .ToListAsync();
    }

    public async Task DeleteBeing(int primaryKey)
    {
        var beingInDb = await FindBeing(primaryKey);
        _context.Beings.Remove(beingInDb);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateBeing(Being updatedBeing)
    {
        var beingInDb = await FindBeing(updatedBeing.PrimaryKey);
        beingInDb = updatedBeing;
        await _context.SaveChangesAsync();
    }
}
