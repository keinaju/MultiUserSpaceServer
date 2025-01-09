using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Generic;

public class ExploreCommand : IGameCommand
{
    public string HelpText => "Explores the curiosity of the current room.";

    public Regex Regex => new("^explore$");

    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public ExploreCommand(
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
            await Explore()
        );
    }

    private async Task<CommandResult> Explore()
    {
        if(_session.AuthenticatedUser is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }
        else
        {
            return await _session.AuthenticatedUser.Explore();
        }
    }
}
