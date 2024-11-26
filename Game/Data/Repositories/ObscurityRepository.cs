using Microsoft.EntityFrameworkCore;
using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public class ObscurityRepository : IObscurityRepository
{
    private readonly GameContext _context;

    public ObscurityRepository(GameContext context)
    {
        _context = context;
    }

    public async Task<Obscurity> FindObscurity(int primaryKey)
    {
        return await _context.Obscurities
            .Include(obscurity => obscurity.RoomPool)
            .ThenInclude(roomPool => roomPool.RoomsInPool)
            .ThenInclude(roomsInPool => roomsInPool.Room)
            .SingleAsync(obscurity => obscurity.PrimaryKey == primaryKey);
    }
}
