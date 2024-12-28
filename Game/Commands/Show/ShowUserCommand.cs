using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Session;

namespace MUS.Game.Commands.Show;

public class ShowUserCommand : IGameCommand
{
    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn
    ];

    public string HelpText =>
    "Shows information about the user that is signed in.";

    public Regex Regex => new("^show user$");

    private User User => _session.AuthenticatedUser!;

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
        _response.AddList(User.Show());

        return Task.CompletedTask;
    }
}
