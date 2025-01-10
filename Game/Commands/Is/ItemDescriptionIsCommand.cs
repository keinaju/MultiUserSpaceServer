using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class ItemDescriptionIsCommand : IGameCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Sets the description of an item.";

    public Regex Pattern => new("^item (.+) description is (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 1);
    
    private string DescriptionInInput => _input.GetGroup(this.Pattern, 2);

    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public ItemDescriptionIsCommand(
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
            return await _session.User
            .ItemDescriptionIs(ItemNameInInput, DescriptionInInput);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
