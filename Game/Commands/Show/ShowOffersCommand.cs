using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Show;

public class ShowOffersCommand : IGameCommand
{
    public bool AdminOnly => false;

    public string HelpText => "Shows all offers in the current room.";

    public Regex Pattern => new("^(show|s) offers$");

    private readonly ISessionService _session;

    public ShowOffersCommand(
        ISessionService session
    )
    {
        _session = session;
    }

    public async Task<CommandResult> Run()
    {
        if(_session.User is not null)
        {
            return await _session.User.ShowOffersInCurrentRoom();
        }
        else
        {
            return NotSignedInResult();
        }
    }
}
