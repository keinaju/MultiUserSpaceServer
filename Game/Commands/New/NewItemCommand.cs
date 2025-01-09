using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.New;

public class NewItemCommand : IGameCommand
{
    public string HelpText => "Creates a new item.";

    public Regex Pattern => new("^new item (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 1);
    
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public NewItemCommand(
        IInputCommand input,
        ISessionService session
    )
    {
        _input = input;
        _session = session;
    }

    public async Task<CommandResult> Run()
    {
        if(_session.User is not null)
        {
            return await _session.User.NewItem(ItemNameInInput);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
