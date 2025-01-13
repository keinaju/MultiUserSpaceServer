using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Generic;

public class PingCommand : ICommandPattern, IUserlessCommand
{
    public bool AdminOnly => false;
    
    public string HelpText => "Generates a response for a ping request.";
    
    public Regex Pattern => new("^(ping|test)$");

    private readonly GameContext _context;
    private readonly IGameUptime _uptime;

    public PingCommand(
        GameContext context,
        IGameUptime uptime
    )
    {
        _context = context;
        _uptime = uptime;
    }

    public async Task<CommandResult> Run(User user)
    {
        return await this.Run();
    }

    public async Task<CommandResult> Run()
    {
        return new CommandResult(StatusCode.Success)
        .AddMessage($"{await GetGameName()} responds.")
        .AddMessage($"The server's uptime is {_uptime.GetUptimeText()}.");
    }

    private async Task<string> GetGameName()
    {
        var settings = await _context.GetGameSettings();

        return settings.GameName;
    }
}
