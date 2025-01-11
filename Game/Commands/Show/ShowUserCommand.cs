using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Show;

public class ShowUserCommand : IGameCommand
{
    public bool AdminOnly => false;

    public string HelpText => 
    "Shows information about the user that is signed in.";

    public Regex Pattern => new("^(show|s) (user|u)$");

    private ISessionService _session;

    public ShowUserCommand(
        ISessionService session
    )
    {
        _session = session;
    }

    public Task<CommandResult> Run()
    {
        if(_session.User is not null)
        {
            return Task.FromResult(
                _session.User.ShowUser()
            );
        }
        else
        {
            return Task.FromResult(
                CommandResult.NotSignedInResult()
            );
        }
    }
}
