using MUS.Game.Data.Repositories;

namespace MUS.Game.Commands;

public class TimeCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    private readonly ITickCounterRepository _tickCounterRepository;

    public TimeCommand(ITickCounterRepository tickCounterRepository)
    : base(regex: @"^time$")
    {
        _tickCounterRepository = tickCounterRepository;
    }

    public override async Task<string> Invoke()
    {
        var tickCounter = await _tickCounterRepository.GetTickCount();
        return $"Tick is {tickCounter!.TickCount}.";
    }
}
