
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Clock;

public class TickSaveListener : IGameClockListener
{
    private ITickCounterRepository _tickCounterRepository;

    public TickSaveListener(ITickCounterRepository tickCounterRepository)
    {
        _tickCounterRepository = tickCounterRepository;
    }

    public Task GetTask(object sender, TickEventArgs eventArgs)
    {
        return SaveTickCountToDatabase(eventArgs.TickCount);
    }

    private async Task SaveTickCountToDatabase(ulong tickCount)
    {
        var tickCounter = new TickCounter() { TickCount = tickCount };
        await _tickCounterRepository.SetTickCount(tickCounter);
    }
}
