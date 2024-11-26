namespace MUS.Game.Commands;

public class CommandParser : ICommandParser
{
    private readonly IEnumerable<IGameCommand> _commands;

    public CommandParser(IEnumerable<IGameCommand> commands)
    {
        _commands = commands;
    }

    public IGameCommand? Parse(string userInput)
    {
        foreach (var command in _commands)
        {
            if (command.IsMatch(userInput)) return command;
        }

        //Unknown command
        return null;
    }
}
