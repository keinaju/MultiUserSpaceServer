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

    private readonly IFeatureRepository _featureRepository;

    private string FeatureName => GetParameter(1);

    protected override string Description =>
        "Creates a new feature.";

    public NewFeatureCommand(IFeatureRepository featureRepository)
    : base(regex: @"^new feature (.+)$")
    {
        _featureRepository = featureRepository;
    }

    public override async Task<string> Invoke()
    {
        var featureInDb = await _featureRepository
            .FindFeature(FeatureName);
        if(featureInDb is not null)
        {
            return $"{featureInDb.Name} feature already exists.";
        }

        var newFeature = await _featureRepository
            .CreateFeature(
                new Feature() { Name = FeatureName }
            );

        return MessageStandard.Created("feature", newFeature.Name);
    }
}
