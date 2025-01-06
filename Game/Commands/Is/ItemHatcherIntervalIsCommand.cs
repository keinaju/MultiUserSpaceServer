using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class ItemHatcherIntervalIsCommand : IGameCommand
{
    public string HelpText => "Sets the interval for an item hatcher.";

    public Condition[] Conditions =>
    [
    ];

    public Regex Regex => new(@"^hatcher (.+) interval is (\d+)$");

    private string ItemNameInInput =>
    _input.GetGroup(this.Regex, 1);

    private string IntervalInInput =>
    _input.GetGroup(this.Regex, 2);

    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public ItemHatcherIntervalIsCommand(
        IResponsePayload response,
        IInputCommand input,
        ISessionService session
    )
    {
        _response = response;
        _input = input;
        _session = session;
    }

    public async Task Run()
    {
        _response.AddResult(
            await ItemHatcherIntervalIs()
        );
    }

    private async Task<CommandResult> ItemHatcherIntervalIs()
    {
        if(_session.AuthenticatedUser is not null)
        {
            return await _session.AuthenticatedUser
            .ItemHatcherIntervalIs(ItemNameInInput, IntervalInInput);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
