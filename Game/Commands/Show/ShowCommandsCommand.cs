using System;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowCommandsCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    protected override string Description =>
        "Shows all commands in the application.";

    private readonly IGameResponse _response;
    private readonly IServiceProvider _serviceProvider;

    public ShowCommandsCommand(
        IGameResponse response,
        IServiceProvider serviceProvider
    )
    : base(regex: $"^show commands$")
    {
        _response = response;
        _serviceProvider = serviceProvider;
    }

    public override async Task Invoke()
    {
        var commands = _serviceProvider.GetServices<IGameCommand>();
        
        var commandsList = new List<string>();

        foreach(var command in commands)
        {
            commandsList.Add(command.HelpText);
        }

        _response.AddText(
            MessageStandard.List(commandsList)
        );
    }
}
