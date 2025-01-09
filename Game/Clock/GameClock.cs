using MUS.Game.Data;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Clock;

public class GameClock : BackgroundService
{
    private readonly IServiceProvider _services;

    private TimeSpan DEFAULT_INTERVAL = TimeSpan.FromSeconds(15);

    private ulong _tickCount = 0;

    public GameClock(
        IServiceProvider services
    )
    {
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //Start a repeating timer to increase ticks
        using (var timer = new PeriodicTimer(DEFAULT_INTERVAL))
        {
            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    if(_tickCount == 0)
                    {
                        _tickCount =  await RetrieveTickCountFromDatabase();
                    }
                    
                    await OnTick(
                        new TickEventArgs()
                        { TickCount = _tickCount }
                    );

                    // Update timer interval
                    timer.Period = await RetrieveTickIntervalFromDatabase();
                    
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

        var listeners = scope.ServiceProvider
        .GetRequiredService<IEnumerable<IGameClockListener>>();
        foreach (var service in listeners)
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

    private async Task<TimeSpan> RetrieveTickIntervalFromDatabase()
    {
        using var scope = _services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<GameContext>();
        var settings = await context.GetGameSettings();
        if(settings is not null)
        {
            return TimeSpan.FromSeconds(settings.TickIntervalSeconds);
        }
        else
        {
            return DEFAULT_INTERVAL;
        }
    }
}

public class TickEventArgs : EventArgs
{
    public ulong TickCount { get; set; }
}