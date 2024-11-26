using MUS.Game.Data.Repositories;

namespace MUS.Game.Clock;

public class GameClock : BackgroundService
{
    public const int INTERVAL_MILLISECONDS = 5_000;

    private ulong _tickCount = 0;
    private readonly IServiceProvider _services;

    public GameClock(
        IServiceProvider services
    )
    {
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //Start a repeating timer to increase ticks
        using (var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(INTERVAL_MILLISECONDS)))
        {
            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    if(_tickCount == 0)
                    {
                        _tickCount =  await RetrieveTickCountFromDatabase();
                    }
                    await OnTick(new TickEventArgs() { TickCount = _tickCount });
                    _tickCount++;
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    }

    private async Task OnTick(TickEventArgs eventArgs)
    {
        using var scope = _services.CreateScope();
        foreach (var service in scope.ServiceProvider.GetRequiredService<IEnumerable<IGameClockListener>>())
        {
            await service.GetTask(this, eventArgs);
        }
    }

    private async Task<ulong> RetrieveTickCountFromDatabase()
    {
        using var scope = _services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITickCounterRepository>();
        var tickCounterInDb = await repository.GetTickCount();

        //If tick does not exist in database, start from 1
        if (tickCounterInDb is null)
        {
            return 1;
        }

        //If tick exists in database, retrieve it
        return tickCounterInDb.TickCount;
    }
}

public class TickEventArgs : EventArgs
{
    public ulong TickCount { get; set; }
}