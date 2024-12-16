using MUS.Game.Data.Repositories;

namespace MUS.Game.Commands;

public class TimeCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    protected override string Description =>
    "Shows the tick count.";

    private readonly IGameResponse _response;
    private readonly ITickCounterRepository _tickCounterRepository;

    public TimeCommand(
        IGameResponse response,
        ITickCounterRepository tickCounterRepository
    )
    : base(regex: @"^time$")
    {
        _response = response;
        _tickCounterRepository = tickCounterRepository;
    }

    public override async Task Invoke()
    {
        var tickCounter = await _tickCounterRepository.GetTickCount();
        _response.AddText(
            $"The tick is {tickCounter!.TickCount.ToString()}."
        );
    }
}
