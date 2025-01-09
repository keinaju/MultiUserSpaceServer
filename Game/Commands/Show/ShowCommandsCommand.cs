using System;
using System.Text.RegularExpressions;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Show;

public class ShowCommandsCommand : IGameCommand
{
    public string HelpText => "Shows all commands.";

    public Regex Regex => new("^(help|show commands|s commands)$");

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
        _response.AddResult(
            ShowCommands()
        );

        return Task.CompletedTask;
    }

    private CommandResult ShowCommands()
    {
        return new CommandResult(StatusCode.Success)
        .AddMessage("All commands are:")
        .AddMessages(GetCommandsList());
    }

    private ICollection<string> GetCommandsList()
    {
        var commands = _serviceProvider.GetServices<IGameCommand>();
        
        var commandsList = new List<string>();
        foreach(var command in commands)
        {
            commandsList.Add(
                $"{command.Regex} = {command.HelpText}"
            );
        }
        commandsList.Sort();

        return commandsList;
    }
}
