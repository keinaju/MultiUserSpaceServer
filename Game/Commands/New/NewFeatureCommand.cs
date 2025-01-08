using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.New;

public class NewFeatureCommand : IGameCommand
{
    public string HelpText => "Creates a new feature.";

    public Condition[] Conditions =>
    [
    ];

    public Regex Regex => new("^new feature (.+)$");

    private string FeatureNameInInput => _input.GetGroup(this.Regex, 1);
    
    private readonly IInputCommand _input;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public NewFeatureCommand(
        IInputCommand input,
        IResponsePayload response,
        ISessionService session
    )
    {
        _input = input;
        _response = response;
        _session = session;
    }

    public async Task Run()
    {
        _response.AddResult(
            await NewFeature()
        );
    }

    private async Task<CommandResult> NewFeature()
    {
        if(_session.AuthenticatedUser is not null)
        {
            return await _session.AuthenticatedUser
            .NewFeature(FeatureNameInInput);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
