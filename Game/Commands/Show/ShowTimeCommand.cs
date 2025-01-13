using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Show;

public class ShowTimeCommand : ICommandPattern
{
    public bool AdminOnly => false;

    public string HelpText => "Shows game's time.";

    public Regex Pattern => new("^(show|s) time$");

    private readonly GameContext _context;

    public ShowTimeCommand(GameContext context)
    {
        _context = context;
    }

    public async Task<CommandResult> Run(User user)
    {
        var tick = await _context.GetTickCount();
        var settings = await _context.GetGameSettings();

        return new CommandResult(StatusCode.Success)
        .AddMessage(
            $"The current tick number is: {tick}." +
            $" The tick interval is {settings.TickIntervalSeconds} seconds."
        );
    }
}
