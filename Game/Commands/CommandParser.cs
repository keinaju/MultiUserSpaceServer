using System;
using MUS.Game.Commands;

namespace MUS.Game.Commands;

public class CommandParser : ICommandParser
{
    private readonly IEnumerable<IGameCommand> _commands;
    private readonly IInputCommand _input;

    public CommandParser(
        IEnumerable<IGameCommand> commands,
        IInputCommand input
    )
    {
        _commands = commands;
        _input = input;
    }

    public IEnumerable<IGameCommand> GetMatchingCommands()
    {
        var matchingCommands = new List<IGameCommand>();
        foreach(var command in _commands)
        {
            var match = command.Pattern.Match(_input.Text);
            if(match.Success)
            {
                matchingCommands.Add(command);
            }
        }

        return matchingCommands;
    }
}
