using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Show;

public class ShowRoomCommand : IGameCommand
{
    public string HelpText => "Shows the current room.";

    public Regex Regex => new("^(show|s) (room|r)$");

    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public ShowRoomCommand(
        IResponsePayload response,
        ISessionService session
    )
    {
        _response = response;
        _session = session;
    }

    public async Task Run()
    {
        _response.AddResult(
            ShowRoom()
        );
    }

    private CommandResult ShowRoom()
    {
        if(_session.AuthenticatedUser is not null)
        {
            return _session.AuthenticatedUser.ShowRoom();
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
