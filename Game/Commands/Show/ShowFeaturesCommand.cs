using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowFeaturesCommand : IGameCommand
{
    public string HelpText => "Shows all features.";

    public Condition[] Conditions => [];

    public Regex Regex => new("^(show|s) features$");

    private readonly IFeatureRepository _featureRepo;
    private readonly IResponsePayload _response;

    public ShowFeaturesCommand(
        IFeatureRepository featureRepo,
        IResponsePayload response
    )
    {
        _featureRepo = featureRepo;
        _response = response;
    }

    public async Task Run()
    {
        var features = await _featureRepo.FindFeatures();

        if(features.Count == 0)
        {
            _response.AddText("There are no features.");
            return;
        }

        _response.AddText("All features are:");

        _response.AddText(GetFeatureNames(features));
    }

    private string GetFeatureNames(IEnumerable<Feature> features)
    {
        var featureNames = new List<string>();

        foreach(var feature in features)
        {
            featureNames.Add(feature.Name);
        }

        featureNames.Sort();

        return Message.List(featureNames);
    }
}
