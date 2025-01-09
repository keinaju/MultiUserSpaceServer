using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class RoomPoolFeeIsCommand : IGameCommand
{
    public string HelpText => "Sets the fee item of a room pool.";

    public Regex Pattern => new("^pool (.+) fee is (.+)$");

    private string RoomPoolNameInInput => _input.GetGroup(this.Pattern, 1);

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 2);

    private readonly GameContext _context;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public RoomPoolFeeIsCommand(
        GameContext context,
        IInputCommand input,
        ISessionService session
    )
    {
        _context = context;
        _input = input;
        _session = session;
    }

    public async Task<CommandResult> Run()
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

        if(_session.User is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }
        else
        {
            return await _session.User.RoomPoolFeeIs(pool, item);
        }
    }
}
