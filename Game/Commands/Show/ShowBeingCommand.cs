using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Show;

public class ShowBeingCommand : IGameCommand
{
    public bool AdminOnly => false;

    public string HelpText => "Shows the currently selected being.";

    public Regex Pattern => new("^(show|s) being$");

    private readonly ISessionService _session;

    public ShowBeingCommand(
        ISessionService session
    )
    {
        _session = session;
    }

    public Task<CommandResult> Run()
    {
        if (_session.User is not null)
        {
            return Task.FromResult(
                _session.User.ShowBeing()
            );
        }
        else
        {
            return Task.FromResult(
                CommandResult.UserIsNotSignedIn()
            );
        }
    }
}
