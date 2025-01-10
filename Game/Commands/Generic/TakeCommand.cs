using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Generic;

public class TakeCommand : IGameCommand
{
    public bool AdminOnly => false;

    public string HelpText => "Takes items from the current room's inventory.";

    public Regex Pattern => new("^take (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public TakeCommand(
        IInputCommand input,
        ISessionService session
    )
    {
        _input = input;
        _session = session;
    }

    public async Task<CommandResult> Run()
    {
        if(_session.User is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }
        else
        {
            return await _session.User.TakeItem(ItemNameInInput);
        }
    }
}
