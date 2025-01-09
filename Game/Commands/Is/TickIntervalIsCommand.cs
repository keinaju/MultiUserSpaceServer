using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Is;

public class TickIntervalIsCommand : IGameCommand
{
    public string HelpText => "Sets the tick interval of the game.";

    public Regex Pattern => new(@"^tick interval is (\d+)$");

    private const int MINIMUM_INTERVAL_SECONDS = 5;

    private string IntervalInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public TickIntervalIsCommand(
        IInputCommand input,
        ISessionService session
    )
    {
        _input = input;
        _session = session;
    }

    public async Task<CommandResult> Run()
    {
        bool ok = int.TryParse(IntervalInInput, out int intervalSeconds);
        if(!ok || intervalSeconds < MINIMUM_INTERVAL_SECONDS)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(
                Message.Invalid(IntervalInInput, "tick interval")
            );
        }

        if(_session.User is not null)
        {
            return await _session.User.SetTickInterval(intervalSeconds);
        }
        else
        {
            return UserIsNotSignedIn();
        }
    }
}
