using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Show;

public class ShowUserCommand : IGameCommand
{
    public string HelpText =>
    "Shows information about the user that is signed in.";

    public Regex Regex => new("^(show|s) (user|u)$");

    private IResponsePayload _response;
    private ISessionService _session;

    public ShowUserCommand(
        IResponsePayload response,
        ISessionService session
    )
    {
        _response = response;
        _session = session;
    }

    public Task Run()
    {
        _response.AddResult(ShowUser());

        return Task.CompletedTask;
    }

    private CommandResult ShowUser()
    {
        if(_session.AuthenticatedUser is not null)
        {
            return _session.AuthenticatedUser.ShowUser();
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
