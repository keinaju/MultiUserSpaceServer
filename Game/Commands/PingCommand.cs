using System;

namespace MUS.Game.Commands;

public class PingCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    protected override string Description => "Tests the server response.";

    private readonly IGameResponse _response;

    public PingCommand(IGameResponse response)
    : base(regex: @"^(ping|test)$")
    {
        _response = response;    
    }

    public override Task Invoke()
    {
        _response.AddText("MUS application responds.");
        return Task.CompletedTask;
    }
}
