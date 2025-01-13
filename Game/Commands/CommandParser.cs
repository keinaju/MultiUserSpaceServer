using System;

namespace MUS.Game.Commands;

public class CommandParser : ICommandParser
{
    private readonly ICommandProvider _commandCollection;
    private readonly IInputCommand _input;

    public CommandParser(
        ICommandProvider commandProvider,
        IInputCommand input
    )
    {
        _commandCollection = commandProvider;
        _input = input;
    }

    public IEnumerable<ICommandPattern> GetMatchingCommands()
    {
        var matchingCommands = new List<ICommandPattern>();

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
