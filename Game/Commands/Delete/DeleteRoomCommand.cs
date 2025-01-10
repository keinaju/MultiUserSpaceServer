using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Delete;

public class DeleteRoomCommand : IGameCommand
{
    public bool AdminOnly => true;
    
    public string HelpText => "Deletes a room.";

    public Regex Pattern => new("^delete room (.+)$");

    private string RoomNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public DeleteRoomCommand(
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
            return UserIsNotSignedIn();
        }
        else
        {
            return await _session.User.DeleteRoom(RoomNameInInput);
        }
    }
}
