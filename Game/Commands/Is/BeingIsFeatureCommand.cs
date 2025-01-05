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

    public Regex Regex => new("^being is (.+)$");

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
        // var feature =
        // await _featureRepo.FindFeature(FeatureNameInInput);

        // if(feature is null)
        // {
        //     _response.AddText(
        //         Message.DoesNotExist("feature", FeatureNameInInput)
        //     );

        //     return;
        // }

        // if(CurrentBeing.HasFeature(feature))
        // {
        //     _response.AddText(
        //         $"{CurrentBeing.Name} already has {feature.Name} feature."
        //     );

        //     return;
        // }

        // await AddFeatureInBeing(feature);

        // _response.AddText(
        //     $"{CurrentBeing.Name} now has {feature.Name} feature."
        // );
    }

    private async Task<CommandResult> BeingIsFeature()
    {
        if(_session.AuthenticatedUser is not null)
        {
            return await _session.AuthenticatedUser
            .BeingIsFeature(FeatureNameInInput);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }

    // private async Task AddFeatureInBeing(Feature feature)
    // {
    //     CurrentBeing.Features.Add(feature);

    //     await _beingRepo.UpdateBeing(CurrentBeing);
    // }
}
