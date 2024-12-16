using System;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.New;

public class NewFeatureCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder
    ];

    protected override string Description => "Creates a new feature.";

    private string FeatureName => GetParameter(1);

    private readonly IGameResponse _response;
    private readonly IFeatureRepository _featureRepository;

    public NewFeatureCommand(
        IFeatureRepository featureRepository,
        IGameResponse response
    )
    : base(regex: @"^new feature (.+)$")
    {
        _featureRepository = featureRepository;
        _response = response;
    }

    public override async Task Invoke()
    {
        var featureInDb = await _featureRepository
            .FindFeature(FeatureName);
        if(featureInDb is not null)
        {
            _response.AddText(
                $"{featureInDb.Name} feature already exists."
            );
            return;
        }

        var newFeature = await _featureRepository
        .CreateFeature(
            new Feature() { Name = FeatureName }
        );

        _response.AddText(
            MessageStandard.Created("feature", newFeature.Name)
        );
    }
}
