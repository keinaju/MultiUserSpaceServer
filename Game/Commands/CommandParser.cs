using System;
using MUS.Game.Commands;

namespace MUS.Game.Commands;

public class CommandParser : ICommandParser
{
    private readonly IEnumerable<IGameCommand> _commands;
    private readonly IUserInput _userInput;

    public CommandParser(
        IEnumerable<IGameCommand> commands,
        IUserInput userInput
    )
    {
        _commands = commands;
        _userInput = userInput;
    }

    public IEnumerable<IGameCommand> GetMatchingCommands()
    {
        var matchingCommands = new List<IGameCommand>();
        foreach(var command in _commands)
        {
            var match = command.Regex.Match(_userInput.Text);
            if(match.Success)
            {
                matchingCommands.Add(command);
            }
        }

        return matchingCommands;
    }
}
