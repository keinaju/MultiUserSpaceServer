using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Generic;

public class GoCommand : IGameCommand
{
    public bool AdminOnly => false;
    
    public string HelpText => "Moves a selected being to another room.";

    public Regex Pattern => new("^go (.+)$");

    private string RoomNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public GoCommand(
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
            return CommandResult.NotSignedInResult();
        }
        else
        {
            return await _session.User.Go(RoomNameInInput);
        }
    }
}
