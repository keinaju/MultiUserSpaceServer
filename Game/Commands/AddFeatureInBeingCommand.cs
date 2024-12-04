using System;
using MUS.Game.Data;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands;

public class AddFeatureInBeingCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder,
        Prerequisite.UserHasPickedBeing
    ];

    private readonly IBeingRepository _beingRepository;
    private readonly IFeatureRepository _featureRepository;
    private readonly IPlayerState _state;

    protected override string Description =>
        "Adds a feature in the current being.";

    private string FeatureName => GetParameter(1);

    public AddFeatureInBeingCommand(
        IBeingRepository beingRepository,
        IFeatureRepository featureRepository,
        IPlayerState state
    )
    : base(regex: @"^add feature (.+) in being$")
    {
        _beingRepository = beingRepository;
        _featureRepository = featureRepository;
        _state = state;
    }

    public override async Task<string> Invoke()
    {
        var feature = await _featureRepository
            .FindFeature(FeatureName);
        if(feature is null)
        {
            return MessageStandard.DoesNotExist("Feature", FeatureName);
        }

        var being = await _state.GetBeing();
        if(being.Features.Contains(feature))
        {
            return $"{being.Name} already has the feature {FeatureName}.";
        }

        being.Features.Add(feature);
        await _beingRepository.UpdateBeing(being);

        return $"{being.Name} now has the feature {feature.Name}.";
    }
}
