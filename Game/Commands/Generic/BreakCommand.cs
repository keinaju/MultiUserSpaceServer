using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Generic;

public class BreakCommand : IGameCommand
{
    public bool AdminOnly => false;
    
    public string HelpText => "Breaks an item to it's components.";

    public Regex Pattern => new("^break (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public BreakCommand(
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
            return CommandResult.NotSignedInResult();
        }
        if(_session.User.SelectedBeing is null)
        {
            return _session.User.NoSelectedBeingResult();
        }
        else
        {
            return await _session.User.SelectedBeing
            .TryBreakItem(ItemNameInInput);
        }
    }
}
