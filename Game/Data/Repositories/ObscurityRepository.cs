using Microsoft.EntityFrameworkCore;
using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public class CuriosityRepository : ICuriosityRepository
{
    private readonly GameContext _context;

    public CuriosityRepository(GameContext context)
    {
        _context = context;
    }

    public async Task<Curiosity> FindCuriosity(int primaryKey)
    {
        return await _context.Curiosities
            .Include(curiosity => curiosity.RoomPool)
            .ThenInclude(roomPool => roomPool.RoomsInPool)
            .ThenInclude(roomsInPool => roomsInPool.Room)
            .SingleAsync(curiosity => curiosity.PrimaryKey == primaryKey);
    }
}
