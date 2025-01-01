using MUS.Game.Commands;

namespace MUS.Game;

public class GameService : IGameService
{
    private readonly ICommandParser _parser;
    private readonly IConditionFilter _filter;
    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;

    public GameService(
        ICommandParser parser,
        IConditionFilter filter,
        IResponsePayload response,
        IInputCommand input
    )
    {
        _filter = filter;
        _parser = parser;
        _response = response;
        _input = input;
    }

    public async Task Respond()
    {
        // Test all commands for matches
        var commands = _parser.GetMatchingCommands();

        if(commands.Count() == 0)
        {
            _response.AddText(
                $"'{_input.Text}' does not match any known command."
            );
        }
        else if(commands.Count() == 1)
        {
            var command = commands.First();

            if(_filter.MeetsConditions(command.Conditions))
            {
                await command.Run();
            }
        }
        else
        {
            _response.AddText(
                $"'{_input.Text}' matches with " +
                $"{commands.Count()} different commands:"
            );
            foreach(var command in commands)
            {
                _response.AddText(
                    $"{command.Regex} = {command.HelpText}"
                );
            }
        }
    }
}
