using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Show;

public class ShowRoomCommand : IGameCommand
{
    public bool AdminOnly => false;

    public string HelpText => "Shows the current room.";

    public Regex Pattern => new("^(show|s) (room|r)$");

    private readonly ISessionService _session;

    public ShowRoomCommand(
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
                _session.User.ShowRoom()
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
