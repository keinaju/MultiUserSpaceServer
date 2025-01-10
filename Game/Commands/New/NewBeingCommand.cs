using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.New;

public class NewBeingCommand : IGameCommand
{
    public bool AdminOnly => false;

    public string HelpText => "Creates a new being.";

    public Regex Pattern => new("^new being (.+)$");

    private string BeingNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public NewBeingCommand(
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
            return await _session.User.NewBeing(BeingNameInInput);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
