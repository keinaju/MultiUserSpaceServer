using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Show;

public class ShowOffersCommand : IGameCommand
{
    public string HelpText => "Shows all offers in the current room.";

    public Regex Regex => new("^(show|s) offers$");

    private readonly GameContext _context;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public ShowOffersCommand(
        GameContext context,
        IResponsePayload response,
        ISessionService session
    )
    {
        _context = context;
        _response = response;
        _session = session;
    }

    public async Task Run()
    {
        _response.AddResult(
            await ShowOffers()
        );
    }

    private async Task<CommandResult> ShowOffers()
    {
        if(_session.AuthenticatedUser is not null)
        {
            return await _session.AuthenticatedUser.ShowOffersInCurrentRoom();
        }
        else
        {
            return UserIsNotSignedIn();
        }
    }
}
