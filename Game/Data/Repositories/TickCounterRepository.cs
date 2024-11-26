using Microsoft.EntityFrameworkCore;
using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public class TickCounterRepository : ITickCounterRepository
{
    private readonly GameContext _context;

    public TickCounterRepository(GameContext context)
    {
        _context = context;
    }

    public async Task<TickCounter?> GetTickCount()
    {
        try
        {
            return await _context.TickCounter.FirstAsync();
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    public async Task SetTickCount(TickCounter tickCounter)
    {
        var tickCounterInDb = await GetTickCount();

        if (tickCounterInDb is null)
        {
            await _context.TickCounter.AddAsync(tickCounter);
        }
        else
        {
            tickCounterInDb.TickCount = tickCounter.TickCount;
        }

        await _context.SaveChangesAsync();
    }
}
