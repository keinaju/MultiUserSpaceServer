using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Generic;

public class SelectBeingCommand : IGameCommand
{
    public bool AdminOnly => false;
    
    public string HelpText => "Selects a being to use.";

    public Regex Pattern => new("^select (.+)$");

    private string BeingNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly ISessionService _session;
    private readonly IInputCommand _input;

    public SelectBeingCommand(
        ISessionService session,
        IInputCommand input
    )
    {
        _session = session;
        _input = input;
    }

    public async Task<CommandResult> Run()
    {
        if(_session.User is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }
        else
        {
            return await _session.User.SelectBeing(BeingNameInInput);
        }
    }
}
