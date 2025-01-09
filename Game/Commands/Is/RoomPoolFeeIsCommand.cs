using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class RoomPoolFeeIsCommand : IGameCommand
{
    public string HelpText => "Sets the fee item of a room pool.";

    public Regex Regex => new("^pool (.+) fee is (.+)$");

    private string RoomPoolNameInInput => _input.GetGroup(this.Regex, 1);

    private string ItemNameInInput => _input.GetGroup(this.Regex, 2);

    private readonly GameContext _context;
    private readonly IInputCommand _input;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public RoomPoolFeeIsCommand(
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
            await RoomPoolFeeIs()
        );
    }

    private async Task<CommandResult> RoomPoolFeeIs()
    {
        var pool = await _context.FindRoomPool(RoomPoolNameInInput);
        if(pool is null)
        {
            return CommandResult.RoomPoolDoesNotExist(RoomPoolNameInInput);
        }
        
        var item = await _context.FindItem(ItemNameInInput);
        if(item is null)
        {
            return CommandResult.ItemDoesNotExist(ItemNameInInput);
        }

        if(_session.AuthenticatedUser is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }

        return await _session.AuthenticatedUser
        .RoomPoolFeeIs(pool, item);
    }
}
