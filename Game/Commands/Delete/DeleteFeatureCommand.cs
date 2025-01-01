using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Delete;

public class DeleteFeatureCommand : IGameCommand
{
    public string HelpText => "Deletes a feature.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder
    ];

    public Regex Regex => new("^delete feature (.+)$");

    private string FeatureNameInInput => _input.GetGroup(this.Regex, 1);

    private readonly IFeatureRepository _featureRepo;
    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;

    public DeleteFeatureCommand(
        IFeatureRepository featureRepo,
        IResponsePayload response,
        IInputCommand input
    )
    {
        _featureRepo = featureRepo;
        _response = response;
        _input = input;
    }

    public async Task Run()
    {
        if(await IsValid())
        {
            await DeleteFeature();
            Respond();
        }
    }

    private async Task<bool> IsValid()
    {
        var feature = await _featureRepo.FindFeature(FeatureNameInInput);
        if(feature is null)
        {
            _response.AddText(
                Message.DoesNotExist("feature", FeatureNameInInput)
            );
            return false;
        }

        return true;
    }

    private async Task DeleteFeature()
    {
        var feature = await _featureRepo.FindFeature(FeatureNameInInput);

        await _featureRepo.DeleteFeature(feature!.PrimaryKey);
    }

    private void Respond()
    {
        _response.AddText(
            Message.Deleted("feature", FeatureNameInInput)
        );
    }
}
