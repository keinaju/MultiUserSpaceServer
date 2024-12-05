using System;
using System.Diagnostics;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowFeaturesCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    private readonly IFeatureRepository _featureRepository;

    protected override string Description =>
        "Shows all features.";

    public ShowFeaturesCommand(IFeatureRepository featureRepository)
    : base(regex: @"^show features$")
    {
        _featureRepository = featureRepository;
    }

    public override async Task<string> Invoke()
    {
        var features = await _featureRepository.FindFeatures();
        if(features.Count == 0)
        {
            return "There are no features.";
        }

        var featureNames = new List<string>();
        foreach(var feature in features)
        {
            featureNames.Add(feature.Name);
        }

        return $"Features are: {MessageStandard.List(featureNames)}.";
    }
}
