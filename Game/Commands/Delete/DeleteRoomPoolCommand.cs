using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Delete;

public class DeleteRoomPoolCommand : IGameCommand
{
    public bool AdminOnly => true;
    
    public string HelpText => "Deletes a room pool.";

    public Regex Pattern => new("^delete pool (.+)$");

    private string RoomPoolNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public DeleteRoomPoolCommand(
        IInputCommand input,
        ISessionService session
    )
    {
        _input = input;
        _session = session;
    }

    public async Task<CommandResult> Run()
    {
        if(_session.User is null)
        {
            return NotSignedInResult();
        }
        else
        {
            return await _session.User.DeleteRoomPool(RoomPoolNameInInput);
        }
    }
}
