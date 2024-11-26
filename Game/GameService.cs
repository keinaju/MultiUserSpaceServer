using MUS.Game.Commands;

namespace MUS.Game;

public class GameService : IGameService
{
    ICommandParser _parser;
    IPrerequisiteFilter _prerequisiteFilter;

    public GameService(ICommandParser parser, IPrerequisiteFilter prerequisiteFilter)
    {
        _parser = parser;
        _prerequisiteFilter = prerequisiteFilter;
    }

    public async Task<string> ProcessUserInput(string userInput)
    {
        var operation = _parser.Parse(userInput);
        if(operation is null)
        {
            return $"'{userInput}' does not match any known operation.";
        }

        string? complain = _prerequisiteFilter.Complain(operation.Prerequisites);
        if (complain is not null) 
        {
            return complain;
        }

        string outcome = await operation.Invoke();
        return outcome;
    }
}
