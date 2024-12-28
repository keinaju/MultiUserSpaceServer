using System;
using System.Text.RegularExpressions;

namespace MUS.Game.Commands.Generic;

public class PingCommand : IGameCommand
{
    public Condition[] Conditions => [];

    public string HelpText => "Tests server response.";
    
    public Regex Regex => new("^(ping|test)$");

    private readonly IResponsePayload _response;

    public PingCommand(IResponsePayload response)
    {
        _response = response;
    }

    public Task Run()
    {
        _response.AddText("MUS application responds.");
        
        return Task.CompletedTask;
    }
}
