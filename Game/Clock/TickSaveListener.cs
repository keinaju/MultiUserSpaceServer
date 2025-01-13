using MUS.Game.Data;

namespace MUS.Game.Clock;

public class TickSaveListener : IGameClockListener
{
    private readonly GameContext _context;

    public TickSaveListener(GameContext context)
    {
        _context = context;
    }

    public Task GetTask(object sender, TickEventArgs eventArgs)
    {
        return SaveTickCountToDatabase(eventArgs.TickCount);
    }

    private async Task SaveTickCountToDatabase(ulong tick)
    {
        await _context.SetTickCount(tick);
    }
}
