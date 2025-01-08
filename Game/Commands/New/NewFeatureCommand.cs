using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.New;

public class NewFeatureCommand : IGameCommand
{
    public string HelpText => "Creates a new feature.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder
    ];

    public Regex Regex => new("^new feature$");
    
    private readonly IFeatureRepository _featureRepo;
    private readonly IResponsePayload _response;

    public NewFeatureCommand(
        IFeatureRepository featureRepo,
        IResponsePayload response
    )
    {
        _featureRepo = featureRepo;
        _response = response;
    }

    public async Task Run()
    {
        var feature = await CreateFeature();

        _response.AddText(
            Message.Created("feature", feature.Name)
        );
    }

    private async Task<Feature> CreateFeature()
    {
        return await _featureRepo.CreateFeature(
            new Feature()
            {
                Name = await _featureRepo.GetUniqueFeatureName("feature #")
            }
        );
    }
}
