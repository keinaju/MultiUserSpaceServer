using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;

namespace MUS.Game.Data.Repositories;

public class BeingRepository : IBeingRepository
{
    private readonly GameContext _context;

    public BeingRepository(GameContext context)
    {
        _context = context;
    }

    public async Task<bool> BeingNameIsReserved(string beingName)
    {
        if(await FindBeing(beingName) is not null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<Being> CreateBeing(Being being)
    {
        being.Name = await GetUniqueBeingName(being.Name);

        EntityEntry<Being> entry =
        await _context.Beings.AddAsync(being);

        await _context.SaveChangesAsync();
        
        return entry.Entity;
    }

    public async Task DeleteBeing(int primaryKey)
    {
        var beingInDb = await FindBeing(primaryKey);
        
        _context.Beings.Remove(beingInDb);

        await _context.SaveChangesAsync();
    }

    public async Task<Being> FindBeing(int primaryKey)
    {
        return await _context.Beings
        .SingleAsync(being => being.PrimaryKey == primaryKey);
    }

    public async Task<Being?> FindBeing(string beingName)
    {
        try
        {
            return await _context.Beings
            .SingleAsync(being => being.Name == beingName);
        }
        catch(InvalidOperationException)
        {
            return null;
        }
    }

    public async Task<Being?> FindBeingByRoomInside(Room room)
    {
        try
        {
            return await _context.Beings
            .SingleAsync(being =>
                being.RoomInside == room
            );
        }
        catch(InvalidOperationException)
        {
            return null;
        }
    }

    public async Task<List<Being>> FindBeingsByUser(User user)
    {
        return await _context.Beings
        .Where(e => e.CreatedByUser == user)
        .ToListAsync();
    }

    public async Task<string> GetUniqueBeingName(string beingName)
    {
        while(await BeingNameIsReserved(beingName))
        {
            beingName += StringUtilities.GetRandomCharacter();
        }

        return beingName;
    }

    public async Task UpdateBeing(Being updatedBeing)
    {
        var beingInDb = await FindBeing(updatedBeing.PrimaryKey);

        beingInDb = updatedBeing;

        await _context.SaveChangesAsync();
    }
}
