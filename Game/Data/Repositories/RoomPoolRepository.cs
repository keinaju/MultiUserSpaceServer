using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public class RoomPoolRepository : IRoomPoolRepository
{
    private readonly GameContext _context;

    public RoomPoolRepository(GameContext context)
    {
        _context = context;
    }

    public async Task<RoomPool> CreateRoomPool(RoomPool roomPool)
    {
        EntityEntry<RoomPool> roomPoolEntry =
            await _context.RoomPools.AddAsync(roomPool);

        await _context.SaveChangesAsync();

        return roomPoolEntry.Entity;
    }

    public async Task<ICollection<RoomPool>> FindRoomPools()
    {
        return await _context.RoomPools.ToListAsync();
    }

    public async Task<RoomPool> FindRoomPool(int primaryKey)
    {
        return await _context.RoomPools
            .Include(roomPool => roomPool.RoomsInPool)
            .ThenInclude(roomsInPool => roomsInPool.Room)
            .Include(roomPool => roomPool.ItemToExplore)
            .SingleAsync(rp => rp.PrimaryKey == primaryKey);
    }

    public async Task<RoomPool?> FindRoomPool(string roomPoolName)
    {
        try
        {
            return await _context.RoomPools
                .Include(rp => rp.RoomsInPool)
                .ThenInclude(rooms => rooms.Room)
                .Include(roomPool => roomPool.ItemToExplore)
                .SingleAsync(rp => rp.Name == roomPoolName);
        }
        catch(InvalidOperationException)
        {
            return null;
        }
    }

    public async Task UpdateRoomPool(RoomPool updatedRoomPool)
    {
        var rpInDb = await FindRoomPool(updatedRoomPool.PrimaryKey);
        rpInDb = updatedRoomPool;
        await _context.SaveChangesAsync();
    }
}
