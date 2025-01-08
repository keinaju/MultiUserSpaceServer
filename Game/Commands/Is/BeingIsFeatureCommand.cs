using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class BeingIsFeatureCommand : IGameCommand
{
    public string HelpText =>
    "Adds a feature in the current being.";

    public Condition[] Conditions =>
    [
    ];

    public Regex Regex => new("^being feature is (.+)$");

    private string FeatureNameInInput =>
    _input.GetGroup(this.Regex, 1);

    private readonly IInputCommand _input;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public BeingIsFeatureCommand(
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
            await BeingIsFeature()
        );
    }

    private async Task<CommandResult> BeingIsFeature()
    {
        if(_session.AuthenticatedUser is not null)
        {
            return await _session.AuthenticatedUser
            .SelectedBeingIsFeature(FeatureNameInInput);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
