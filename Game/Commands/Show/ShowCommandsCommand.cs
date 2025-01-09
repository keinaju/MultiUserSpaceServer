using System;
using System.Text.RegularExpressions;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Show;

public class ShowCommandsCommand : IGameCommand
{
    public string HelpText => "Shows all commands.";

    public Regex Pattern => new("^help$");

    private readonly IServiceProvider _serviceProvider;

    public ShowCommandsCommand(
        IServiceProvider serviceProvider
    )
    {
        _serviceProvider = serviceProvider;        
    }

    public Task<CommandResult> Run()
    {
        return Task.FromResult(
            ShowCommands()
        );
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
                $"{command.Pattern} = {command.HelpText}"
            );
        }
        commandsList.Sort();

        return commandsList;
    }
}
