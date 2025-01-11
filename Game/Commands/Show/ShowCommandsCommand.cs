using System;
using System.Text.RegularExpressions;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Show;

public class ShowCommandsCommand : IGameCommand
{
    public bool AdminOnly => false;

    public string HelpText => "Shows all commands.";

    public Regex Pattern => new("^help$");

    private Regex _removableCharacters = new Regex("\\(|\\)|\\^|\\$");
    private Regex _wildcard = new Regex("\\.\\+");
    private Regex _digit = new Regex("\\\\d\\+");

    private readonly ICommandCollection _commandCollection;

    public ShowCommandsCommand(
        ICommandCollection commandCollection
    )
    {
        _commandCollection = commandCollection;
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
        return GetHelpTexts(_commandCollection.GetCommands());
    }

    private List<string> GetHelpTexts(IEnumerable<IGameCommand> commands)
    {
        var helpTexts = new List<string>();

        foreach(var command in commands)
        {
            helpTexts.Add(
                $"{GetReadablePattern(command.Pattern.ToString())} => {command.HelpText}"
            );
        }

        helpTexts.Sort();

        return helpTexts;
    }

    private string GetReadablePattern(string pattern)
    {
        pattern = _removableCharacters.Replace(pattern, "");
        pattern = _wildcard.Replace(pattern, "<...>");
        pattern = _digit.Replace(pattern, "<num>");
        return pattern;
    }
}
