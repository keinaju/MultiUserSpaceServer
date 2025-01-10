using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class FeatureNameIsCommand : IGameCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Renames a feature.";

    public Regex Pattern => new("^feature (.+) name is (.+)$");

    private string OldFeatureNameInInput => _input.GetGroup(this.Pattern, 1);
    
    private string NewFeatureNameInInput => _input.GetGroup(this.Pattern, 2);

    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public FeatureNameIsCommand(
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
            return await _session.User.FeatureNameIs(
                OldFeatureNameInInput, NewFeatureNameInInput
            );
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
