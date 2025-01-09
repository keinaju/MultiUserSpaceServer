using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class BeingIsFeatureCommand : IGameCommand
{
    public string HelpText => "Adds a feature in the current being.";

    public Regex Pattern => new("^being feature is (.+)$");

    private string FeatureNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public BeingIsFeatureCommand(
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
            return await _session.User
            .SelectedBeingIsFeature(FeatureNameInInput);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
