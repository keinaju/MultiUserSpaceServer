using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Generic;

public class ExploreCommand : IGameCommand
{
    public bool AdminOnly => false;
    
    public string HelpText => "Explores the curiosity of the current room.";

    public Regex Pattern => new("^explore$");

    private readonly ISessionService _session;

    public ExploreCommand(
        ISessionService session
    )
    {
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
            return await _session.User.SelectedBeing.Explore();
        }
    }
}
