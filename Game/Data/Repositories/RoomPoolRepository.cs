using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;

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
        .SingleAsync(rp => rp.PrimaryKey == primaryKey);
    }

    public async Task<RoomPool?> FindRoomPool(string roomPoolName)
    {
        try
        {
            return await _context.RoomPools
            .SingleAsync(rp => rp.Name == roomPoolName);
        }
        catch(InvalidOperationException)
        {
            return null;
        }
    }

    public async Task<string> GetUniqueRoomPoolName(string roomPoolName)
    {
        while(await RoomPoolNameIsReserved(roomPoolName))
        {
            roomPoolName += StringUtilities.GetRandomCharacter();
        }

        return roomPoolName;
    }

    public async Task<bool> RoomPoolNameIsReserved(string roomPoolName)
    {
        if(await FindRoomPool(roomPoolName) is null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public async Task UpdateRoomPool(RoomPool updatedRoomPool)
    {
        var rpInDb = await FindRoomPool(updatedRoomPool.PrimaryKey);

        rpInDb = updatedRoomPool;

        await _context.SaveChangesAsync();
    }
}
