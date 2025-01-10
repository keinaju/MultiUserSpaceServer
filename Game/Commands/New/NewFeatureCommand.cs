using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.New;

public class NewFeatureCommand : IGameCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Creates a new feature.";

    public Regex Pattern => new("^new feature (.+)$");

    private string FeatureNameInInput => _input.GetGroup(this.Pattern, 1);
    
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public NewFeatureCommand(
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
            return await _session.User.NewFeature(FeatureNameInInput);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
