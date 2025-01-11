using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class RoomIsInBeingCommand : IGameCommand
{
    public bool AdminOnly => true;

    public string HelpText =>
    "Sets the current room as an inside room of the current being.";

    public Regex Pattern => new("^room is inside$");
    
    private readonly ISessionService _session;

    public RoomIsInBeingCommand(
        ISessionService session
    )
    {
        _session = session;
    }

    public async Task<CommandResult> Run()
    {
        if(_session.User is not null)
        {
            return await _session.User.RoomIsInBeing();
        }
        else
        {
            return CommandResult.NotSignedInResult();
        }
    }
}
