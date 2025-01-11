using System;

namespace MUS.Game.Commands;

public class CommandParser : ICommandParser
{
    private readonly ICommandCollection _commandCollection;
    private readonly IInputCommand _input;

    public CommandParser(
        ICommandCollection commandCollection,
        IInputCommand input
    )
    {
        _commandCollection = commandCollection;
        _input = input;
    }

    public IEnumerable<IGameCommand> GetMatchingCommands()
    {
        var matchingCommands = new List<IGameCommand>();

        foreach(var command in _commandCollection.GetCommands())
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
