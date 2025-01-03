using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Delete;

public class DeleteFeatureCommand : IGameCommand
{
    public string HelpText => "Deletes a feature.";

    public Condition[] Conditions =>
    [
    ];

    public Regex Regex => new("^delete feature (.+)$");

    private string FeatureNameInInput => _input.GetGroup(this.Regex, 1);

    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public DeleteFeatureCommand(
        IResponsePayload response,
        IInputCommand input,
        ISessionService session
    )
    {
        _response = response;
        _input = input;
        _session = session;
    }

    public async Task Run()
    {
        _response.AddResult(
            await DeleteFeature()
        );
    }

    private async Task<CommandResult> DeleteFeature()
    {
        if(_session.AuthenticatedUser is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }
        else
        {
            return await _session.AuthenticatedUser
            .DeleteFeature(FeatureNameInInput);
        }
    }
}
