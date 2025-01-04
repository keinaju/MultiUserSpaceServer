using System;
using System.Text.RegularExpressions;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Generic;

public class PingCommand : IGameCommand
{
    public Condition[] Conditions => [];

    public string HelpText => "Tests server response.";
    
    public Regex Regex => new("^(ping|test)$");

    private readonly IGameUptime _uptime;
    private readonly IResponsePayload _response;

    public PingCommand(
        IGameUptime uptime,
        IResponsePayload response
    )
    {
        _uptime = uptime;
        _response = response;
    }

    public Task Run()
    {
        _response.AddResult(
            new CommandResult(StatusCode.Success)
            .AddMessage($"The application responds.")
            .AddMessage($"The application's uptime is {_uptime.GetUptimeText()}.")
        );
        
        return Task.CompletedTask;
    }
}
