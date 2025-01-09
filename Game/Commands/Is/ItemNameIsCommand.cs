using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Is;

public class ItemNameIsCommand : IGameCommand
{
    public string HelpText => "Renames an item.";

    public Regex Pattern => new("^item (.+) name is (.+)$");

    private string OldItemNameInInput => _input.GetGroup(this.Pattern, 1);

    private string NewItemNameInInput => _input.GetGroup(this.Pattern, 2);

    private readonly GameContext _context;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public ItemNameIsCommand(
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
        var item = await _context.FindItem(OldItemNameInInput);
        if(item is null)
        {
            return ItemDoesNotExist(OldItemNameInInput);
        }

        if(_session.User is null)
        {
            return UserIsNotSignedIn();
        }
        else
        {
            return await _session.User.ItemNameIs(item, NewItemNameInInput);
        }
    }
}
