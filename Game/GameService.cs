using MUS.Game.Commands;

namespace MUS.Game;

public class GameService : IGameService
{
    private readonly ICommandParser _parser;
    private readonly IGameResponse _response;
    private readonly IPrerequisiteFilter _prerequisiteFilter;

    public GameService(
        ICommandParser parser,
        IGameResponse response,
        IPrerequisiteFilter prerequisiteFilter
    )
    {
        _parser = parser;
        _prerequisiteFilter = prerequisiteFilter;
        _response = response;
    }

    public async Task<IGameResponse> Respond(string userInput)
    {
        var command = _parser.Parse(userInput);
        if(command is null)
        {
            _response.AddText($"'{userInput}' does not match any known command.");
            return _response;
        }

        string? complainText = _prerequisiteFilter.Complain(command.Prerequisites);
        if (complainText is not null) 
        {
            _response.AddText(complainText);
            return _response;
        }

        await command.Invoke();
        return _response;
    }
}
