using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class FeatureNameIsCommand : IGameCommand
{
    public string HelpText => "Renames a feature.";

    public Regex Regex => new("^feature (.+) name is (.+)$");

    private string OldFeatureNameInInput =>
    _input.GetGroup(this.Regex, 1);
    
    private string NewFeatureNameInInput =>
    _input.GetGroup(this.Regex, 2);

    private readonly IInputCommand _input;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public FeatureNameIsCommand(
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
            await FeatureNameIs()
        );
    }

    private async Task<CommandResult> FeatureNameIs()
    {
        if(_session.AuthenticatedUser is not null)
        {
            return await _session.AuthenticatedUser.FeatureNameIs(
                OldFeatureNameInInput, NewFeatureNameInInput
            );
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
