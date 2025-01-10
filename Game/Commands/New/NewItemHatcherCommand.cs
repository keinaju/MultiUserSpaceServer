using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;

namespace MUS.Game.Commands.New;

public class NewItemHatcherCommand : IGameCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Creates a new item hatcher.";

    public Regex Pattern => new("^new hatcher (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly GameContext _context;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public NewItemHatcherCommand(
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
        var item = await _context.FindItem(ItemNameInInput);
        if(item is null)
        {
            return CommandResult.ItemDoesNotExist(ItemNameInInput);
        }

        if(_session.User is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }
        else
        {
            return await _session.User.NewItemHatcher(item);
        }
    }
}
