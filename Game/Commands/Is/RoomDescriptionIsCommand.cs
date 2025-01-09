using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class RoomDescriptionIsCommand : IGameCommand
{
    public string HelpText => "Sets the description of the current room.";

    public Regex Pattern => new("^room description is (.+)$");

    private string DescriptionInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public RoomDescriptionIsCommand(
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
            return await _session.User.RoomDescriptionIs(DescriptionInInput);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
