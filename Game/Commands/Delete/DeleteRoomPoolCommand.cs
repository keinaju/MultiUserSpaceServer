using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;

namespace MUS.Game.Commands.Delete;

public class DeleteRoomPoolCommand : IGameCommand
{
    public bool AdminOnly => true;
    
    public string HelpText => "Deletes a room pool.";

    public Regex Pattern => new("^delete pool (.+)$");

    private string RoomPoolNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly GameContext _context;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public DeleteRoomPoolCommand(
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
        if(_session.User is null)
        {
            return CommandResult.NotSignedInResult();
        }
        else
        {
            return await _context.DeleteRoomPool(RoomPoolNameInInput);
        }
    }
}
