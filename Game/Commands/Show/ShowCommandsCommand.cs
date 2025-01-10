using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Show;

public class ShowCommandsCommand : IGameCommand
{
    public bool AdminOnly => false;

    public string HelpText => "Shows all commands.";

    public Regex Pattern => new("^help$");

    private readonly IServiceProvider _serviceProvider;
    private readonly ISessionService _session;

    public ShowCommandsCommand(
        IServiceProvider serviceProvider,
        ISessionService session
    )
    {
        _serviceProvider = serviceProvider;
        _session = session;
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
            // Do not confuse the player by showing a list 
            // of admin commands, unless the user is admin
            if(command.AdminOnly)
            {
                if (_session.User is null) continue;
                if (!_session.User.IsBuilder) continue;
            }

            commandsList.Add(
                $"{GetReadablePattern(command.Pattern.ToString())} => {command.HelpText}"
            );
        }
        commandsList.Sort();

        return commandsList;
    }

    private string GetReadablePattern(string pattern)
    {
        var removableCharacters = new Regex("\\(|\\)|\\^|\\$");
        var wildcard = new Regex("\\.\\+");
        var digit = new Regex("\\\\d\\+");

        string readablePattern = removableCharacters.Replace(pattern, "");
        readablePattern = wildcard.Replace(readablePattern, "<...>");
        readablePattern = digit.Replace(readablePattern, "<num>");

        return readablePattern;
    }
}
