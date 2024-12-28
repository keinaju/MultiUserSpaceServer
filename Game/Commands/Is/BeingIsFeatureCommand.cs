using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Is;

public class BeingIsFeatureCommand : IGameCommand
{
    public string HelpText =>
    "Adds a feature in the current being.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder,
        Condition.UserHasSelectedBeing
    ];

    public Regex Regex => new("^being is (.+)$");

    private string FeatureNameInInput =>
    _userInput.GetGroup(this.Regex, 1);

    private Being CurrentBeing => _player.GetSelectedBeing();

    private readonly IBeingRepository _beingRepo;
    private readonly IFeatureRepository _featureRepo;
    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IUserInput _userInput;

    public BeingIsFeatureCommand(
        IBeingRepository beingRepo,
        IFeatureRepository featureRepo,
        IPlayerState player,
        IResponsePayload response,
        IUserInput userInput
    )
    {
        _beingRepo = beingRepo;
        _featureRepo = featureRepo;
        _player = player;
        _response = response;
        _userInput = userInput;
    }

    public async Task Run()
    {
        var feature =
        await _featureRepo.FindFeature(FeatureNameInInput);

        if(feature is null)
        {
            _response.AddText(
                Message.DoesNotExist("feature", FeatureNameInInput)
            );

            return;
        }

        if(CurrentBeing.HasFeature(feature))
        {
            _response.AddText(
                $"{CurrentBeing.Name} already has {feature.Name} feature."
            );

            return;
        }

        await AddFeatureInBeing(feature);

        _response.AddText(
            $"{CurrentBeing.Name} now has {feature.Name} feature."
        );
    }

    private async Task AddFeatureInBeing(Feature feature)
    {
        CurrentBeing.Features.Add(feature);

        await _beingRepo.UpdateBeing(CurrentBeing);
    }
}
