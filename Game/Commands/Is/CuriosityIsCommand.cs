using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class CuriosityIsCommand : IGameCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Sets the curiosity of the current room.";

    public Regex Pattern => new("^curiosity is (.+)$");

    private string RoomPoolNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public CuriosityIsCommand(
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
            return await _session.User.CuriosityIs(RoomPoolNameInInput);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
