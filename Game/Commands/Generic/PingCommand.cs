using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Generic;

public class PingCommand : IGameCommand
{
    public string HelpText => "Generates a response for a ping request.";
    
    public Regex Regex => new("^(ping|test)$");

    private readonly GameContext _context;
    private readonly IGameUptime _uptime;
    private readonly IResponsePayload _response;

    public PingCommand(
        GameContext context,
        IGameUptime uptime,
        IResponsePayload response
    )
    {
        _context = context;
        _uptime = uptime;
        _response = response;
    }

    public async Task Run()
    {
        _response.AddResult(
            new CommandResult(StatusCode.Success)
            .AddMessage($"{await GetGameName()} responds.")
            .AddMessage($"The server's uptime is {_uptime.GetUptimeText()}.")
        );
    }

    private async Task<string> GetGameName()
    {
        var settings = await _context.GetGameSettings();

        if(settings is not null)
        {
            return settings.GameName;
        }
        else
        {
            return "MUS-application";
        }
    }
}
