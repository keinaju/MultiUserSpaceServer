using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class RoomIsGlobalCommand : IGameCommand
{
    public bool AdminOnly => true;

    public string HelpText =>
    "Sets the global accessibility of the current room.";

    public Regex Pattern => new("^room (is|is not) global$");

    private string WordInInput => _input.GetGroup(this.Pattern, 1);

    private bool NewValue => WordInInput == "is" ? true : false;

    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public RoomIsGlobalCommand(
        IInputCommand input,
        ISessionService session
    )
    {
        _input = input;
        _session = session;
    }

    public async Task<CommandResult> Run()
    {
        if(_session.User is not null)
        {
            return await _session.User.RoomIsGlobal(NewValue);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
