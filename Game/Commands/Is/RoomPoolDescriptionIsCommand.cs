using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class RoomPoolDescriptionIsCommand : IGameCommand
{
    public string HelpText => "Sets the description of a room pool.";

    public Regex Regex =>
    new("^pool (.+) description is (.+)$");

    private string RoomPoolNameInInput =>
    _input.GetGroup(this.Regex, 1);
    
    private string RoomPoolDescriptionInInput =>
    _input.GetGroup(this.Regex, 2);

    private readonly GameContext _context;
    private readonly IInputCommand _input;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public RoomPoolDescriptionIsCommand(
        GameContext context,
        IInputCommand input,
        IResponsePayload response,
        ISessionService session
    )
    {
        _context = context;
        _input = input;
        _response = response;
        _session = session;
    }

    public async Task Run()
    {
        _response.AddResult(
            await RoomPoolDescriptionIs()
        );
    }

    private async Task<CommandResult> RoomPoolDescriptionIs()
    {
        var pool = await _context.FindRoomPool(RoomPoolNameInInput);
        if(pool is null)
        {
            return CommandResult.RoomPoolDoesNotExist(RoomPoolNameInInput);
        }

        if(_session.AuthenticatedUser is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }

        return await _session.AuthenticatedUser
        .RoomPoolDescriptionIs(pool, RoomPoolDescriptionInInput);
    }
}
