using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Show;

public class ShowTimeCommand : IGameCommand
{
    public string HelpText => "Shows game's time.";

    public Regex Regex => new("^(show|s) time$");

    private readonly GameContext _context;
    private readonly IResponsePayload _response;

    public ShowTimeCommand(
        GameContext context,
        IResponsePayload response
    )
    {
        _context = context;
        _response = response;
    }

    public async Task Run()
    {
        _response.AddResult(await ShowTime());
    }

    private async Task<CommandResult> ShowTime()
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
