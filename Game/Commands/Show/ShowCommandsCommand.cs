using System;
using System.Text.RegularExpressions;

namespace MUS.Game.Commands.Show;

public class ShowCommandsCommand : IGameCommand
{
    public string HelpText => "Shows all commands.";

    public Condition[] Conditions => [];

    public Regex Regex => new("^(help|show commands)$");

    private readonly IResponsePayload _response;
    private readonly IServiceProvider _serviceProvider;

    public ShowCommandsCommand(
        IResponsePayload response,
        IServiceProvider serviceProvider
    )
    {
        _response = response;
        _serviceProvider = serviceProvider;        
    }

    public Task Run()
    {
        var commands = _serviceProvider
        .GetServices<IGameCommand>();
        
        var commandsList = new List<string>();
        foreach(var command in commands)
        {
            commandsList.Add(
                $"{command.Regex} = {command.HelpText}"
            );
        }
        commandsList.Sort();

        _response.AddText("All commands are:");

        _response.AddList(commandsList);

        return Task.CompletedTask;
    }
}
