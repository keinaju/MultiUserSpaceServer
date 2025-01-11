using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class BeingNameIsCommand : IGameCommand
{
    public bool AdminOnly => false;

    public string HelpText => "Renames the currently selected being.";

    public Regex Pattern => new("^being name is (.+)$");

    private string BeingNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public BeingNameIsCommand(
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
            return await _session.User.SelectedBeingNameIs(BeingNameInInput);
        }
        else
        {
            return CommandResult.NotSignedInResult();
        }
    }
}
