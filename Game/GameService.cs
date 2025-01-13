using MUS.Game.Commands;
using MUS.Game.Session;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game;

public class GameService : IGameService
{
    private readonly ICommandParser _parser;
    private readonly IInputCommand _input;
    private readonly IResponsePayload _response;
    private readonly IUserSession _session;

    public GameService(
        ICommandParser parser,
        IInputCommand input,
        IResponsePayload response,
        IUserSession session
    )
    {
        _parser = parser;
        _input = input;
        _response = response;
        _session = session;
    }

    public async Task Respond()
    {
        _response.AddResult(await GetResult());
    }

    private async Task<CommandResult> GetResult()
    {
        // Test command patterns for a match
        var matches = _parser.GetMatchingCommands();

        switch(matches.Count())
        {
            // Input has matched with only one command
            case 1: return await GetSingleMatchResult(matches.First());

            // Input has not matched with any commands
            case 0: return GetNoMatchResult();
            
            // Input has matched with multiple commands
            default: return GetMultipleMatchResult(matches);
        }
    }

    private CommandResult GetNoMatchResult()
    {
        return new CommandResult(StatusCode.Fail)
        .AddMessage(
            $"Input '{_input.Text}' does not match with any command patterns."
        );
    }

    private async Task<CommandResult> GetSingleMatchResult(ICommandPattern command)
    {
        // If user is signed in, run with user
        if (_session.User is not null)
        {
            return await command.Run(_session.User);
        }
        // If user is not signed in but the command does not require a user
        else if (command is IUserlessCommand)
        {
            var userless = (IUserlessCommand)command;
            return await userless.Run();
        }
        else
        {
            return CommandResult.NotSignedInResult();
        }
    }

    private CommandResult GetMultipleMatchResult(
        IEnumerable<ICommandPattern> commands
    )
    {
        var messages = new List<string>();

        messages.Add(
            $"Input '{_input.Text}' matches with {commands.Count()} different commands:"
        );

        foreach(var command in commands)
        {
            messages.Add($"{command.Pattern} = {command.HelpText}");
        }

        return new CommandResult(StatusCode.Fail)
        .AddMessages(messages);
    }
}
