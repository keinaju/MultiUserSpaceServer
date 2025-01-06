using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

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

    private readonly GameContext _context;
    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public ItemHatcherIntervalIsCommand(
        GameContext context,
        IResponsePayload response,
        IInputCommand input,
        ISessionService session
    )
    {
        _context = context;
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
        bool ok = int.TryParse(IntervalInInput, out int interval);
        if(!ok || interval < 1)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(Message.Invalid(IntervalInInput, "interval"));
        }
        
        var item = await _context.FindItem(ItemNameInInput);
        if(item is not null)
        {
            if(_session.AuthenticatedUser is not null)
            {
                return await _session.AuthenticatedUser
                .ItemHatcherIntervalIs(item, interval);
            }
            else
            {
                return UserIsNotSignedIn();
            }
        }
        else
        {
            return ItemDoesNotExist(ItemNameInInput);
        }
    }
}
