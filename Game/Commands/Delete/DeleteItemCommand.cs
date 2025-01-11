using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;

namespace MUS.Game.Commands.Delete;

public class DeleteItemCommand : IGameCommand
{
    public bool AdminOnly => true;
    
    public string HelpText => "Deletes an item.";

    public Regex Pattern => new("^delete item (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly GameContext _context;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public DeleteItemCommand(
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
        if(_session.User is null)
        {
            return CommandResult.NotSignedInResult();
        }
        else
        {
            return await _context.DeleteItem(ItemNameInInput);
        }
    }
}
