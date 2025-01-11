using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Is;

public class TickIntervalIsCommand : IUserCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Sets the tick interval of the game.";

    public Regex Pattern => new(@"^tick interval is (\d+)$");

    private const int MINIMUM_INTERVAL_SECONDS = 5;

    private string IntervalInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;

    public TickIntervalIsCommand(IInputCommand input)
    {
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        bool ok = int.TryParse(IntervalInInput, out int intervalSeconds);
        if(!ok || intervalSeconds < MINIMUM_INTERVAL_SECONDS)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(
                Message.Invalid(IntervalInInput, "tick interval")
            );
        }
        else
        {
            return await user.SetTickInterval(intervalSeconds);
        }
    }
}
