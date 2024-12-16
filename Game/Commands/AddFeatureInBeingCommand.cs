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
        Prerequisite.UserHasSelectedBeing
    ];

    private readonly IBeingRepository _beingRepository;
    private readonly IFeatureRepository _featureRepository;
    private readonly IGameResponse _response;
    private readonly IPlayerState _state;

    protected override string Description =>
        "Adds a feature in the current being.";

    private string FeatureName => GetParameter(1);

    public AddFeatureInBeingCommand(
        IBeingRepository beingRepository,
        IFeatureRepository featureRepository,
        IGameResponse response,
        IPlayerState state
    )
    : base(regex: @"^add feature (.+) in being$")
    {
        _beingRepository = beingRepository;
        _featureRepository = featureRepository;
        _response = response;
        _state = state;
    }

    public override async Task Invoke()
    {
        var feature = await _featureRepository
            .FindFeature(FeatureName);
        if(feature is null)
        {
            _response.AddText(
                MessageStandard.DoesNotExist("Feature", FeatureName)
            );
            return;
        }

        var being = await _state.GetBeing();
        if(being.Features.Contains(feature))
        {
            _response.AddText(
                $"{being.Name} already has the feature {FeatureName}."
            );
            return;
        }

        being.Features.Add(feature);
        await _beingRepository.UpdateBeing(being);

        _response.AddText(
            $"{feature.Name} feature was added to {being.Name}."
        );
    }
}
