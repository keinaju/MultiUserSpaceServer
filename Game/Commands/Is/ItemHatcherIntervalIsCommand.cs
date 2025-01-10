using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Is;

public class ItemHatcherIntervalIsCommand : IGameCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Sets the interval for an item hatcher.";

    public Regex Pattern => new(@"^hatcher (.+) interval is (\d+)$");

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 1);

    private string IntervalInInput => _input.GetGroup(this.Pattern, 2);

    private readonly GameContext _context;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public ItemHatcherIntervalIsCommand(
        GameContext context,
        IInputCommand input,
        ISessionService session
    )
    {
        _context = context;
        _input = input;
        _session = session;
    }

    public async Task<CommandResult> Run()
    {
        bool ok = int.TryParse(IntervalInInput, out int interval);
        if(!ok || interval < 1)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(Message.Invalid(IntervalInInput, "interval"));
        }
        
        var item = await _context.FindItem(ItemNameInInput);
        if(item is null)
        {
            return ItemDoesNotExist(ItemNameInInput);
        }

        if(_session.User is null)
        {
            return UserIsNotSignedIn();
        }
        else
        {
            return await _session.User.ItemHatcherIntervalIs(item, interval);
        }
    }
}